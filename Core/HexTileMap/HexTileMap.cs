using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Godot;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Core;

public partial class HexTileMap : Node2D
{
    [Export]
    public int Width { get; set; } = 20;

    [Export]
    public int Height { get; set; } = 20;

    [Export]
    public int SeedLand { get; set; } = 0;

    [Export]
    public int SeedForest { get; set; } = 0;

    [Export]
    public int SeedDesert { get; set; } = 0;

    [Export]
    public int SeedMountain { get; set; } = 0;

    [Export]
    private PackedScene? CityScene { get; set; }

    [Export]
    private PackedScene? HexOverlayScene { get; set; }

    public Hex? CurrentlySelectedHex { get; set; }

    private TileMapLayer BaseLayer = null!;
    private TileMapLayer BorderLayer = null!;
    private TileMapLayer OverlayLayer = null!;

    private readonly int maxSeed = 100_000;

    private readonly Dictionary<Vector2I, Hex> mapData = [];

    private readonly Dictionary<TerrainType, Vector2I> terrainToTextureCoords = new()
    {
        [TerrainType.Plains] = new(0, 0),
        [TerrainType.Desert] = new(0, 1),
        [TerrainType.Beach] = new(0, 2),
        [TerrainType.Ice] = new(0, 3),
        [TerrainType.Water] = new(1, 0),
        [TerrainType.Mountain] = new(1, 1),
        [TerrainType.Shallows] = new(1, 2),
        [TerrainType.Forest] = new(1, 3),
    };

    public override void _Ready()
    {
        BaseLayer = GetNode<TileMapLayer>("BaseLayer");
        BorderLayer = GetNode<TileMapLayer>("BorderLayer");
        OverlayLayer = GetNode<TileMapLayer>("OverlayLayer");

        GenerateTerrain();
        GD.Print("HexMap Ready!");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouse)
        {
            if (mouse.ButtonMask == MouseButtonMask.Left)
            {
                var mapCoords = BaseLayer.LocalToMap(ToLocal(GetGlobalMousePosition()));
                mapData.TryGetValue(mapCoords, out Hex? hex);
                if (hex is not null)
                {
                    GD.Print($"Clicked Hex: {hex}");
                    if (CurrentlySelectedHex is not null)
                    {
                        OverlayLayer.SetCell(CurrentlySelectedHex.Coords, 0, new Vector2I(0, 0));
                    }
                    OverlayLayer.SetCell(hex.Coords, 0, new Vector2I(0, 1));
                    CurrentlySelectedHex = hex;
                }
            }
            if (mouse.ButtonMask == MouseButtonMask.Right)
            {
                if (CurrentlySelectedHex is not null)
                {
                    OverlayLayer.SetCell(CurrentlySelectedHex.Coords, 0, new Vector2I(0, 0));
                }
                CurrentlySelectedHex = null;
            }
            GlobalEvents.RaiseHexSelected(CurrentlySelectedHex);
        }
    }

    public Vector2 MapToLocal(Vector2I coords)
    {
        return BaseLayer.MapToLocal(coords);
    }

    private void GenerateTerrain()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var r = new Random();
        var noise = CreateNoise(SeedLand == 0 ? r.Next(maxSeed) : SeedLand);
        var noiseForest = CreateNoiseForest(SeedForest == 0 ? r.Next(maxSeed) : SeedForest);
        var noiseDesert = CreateNoiseDesert(SeedDesert == 0 ? r.Next(maxSeed) : SeedDesert);
        var noiseMountain = CreateNoiseMountain(SeedMountain == 0 ? r.Next(maxSeed) : SeedMountain);
        var mapValues = new float[Width, Height];
        var mapValuesForest = new float[Width, Height];
        var mapValuesDesert = new float[Width, Height];
        var mapValuesMountain = new float[Width, Height];

        float max = 0f;
        float maxForest = 0f;
        float maxDesert = 0f;
        float maxMountain = 0f;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float value = Math.Abs(noise.GetNoise2D(x, y));
                mapValues[x, y] = value;
                max = Math.Max(max, value);

                float valueForest = Math.Abs(noiseForest.GetNoise2D(x, y));
                mapValuesForest[x, y] = valueForest;
                maxForest = Math.Max(maxForest, valueForest);

                float valueDesert = Math.Abs(noiseDesert.GetNoise2D(x, y));
                mapValuesDesert[x, y] = valueDesert;
                maxDesert = Math.Max(maxDesert, valueDesert);

                float valueMountain = Math.Abs(noiseMountain.GetNoise2D(x, y));
                mapValuesMountain[x, y] = valueMountain;
                maxMountain = Math.Max(maxMountain, valueMountain);
            }
        }
        stopwatch.Stop();

        GD.Print($"Terrain data generation took: {stopwatch.ElapsedMilliseconds} ms.");

        GD.Print(
            $"max={max}, maxForest={maxForest}, maxDesert={maxDesert}, maxMountain={maxMountain}"
        );

        stopwatch.Restart();

        int iceDepth = 5;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var coords = new Vector2I(x, y);
                var terrain = GetBaseTerrain(mapValues[x, y], max);
                var hex = new Hex(coords) { TerrainType = terrain };
                if (
                    hex.TerrainType is TerrainType.Plains
                    && IsDesert(mapValuesDesert[x, y], maxDesert)
                )
                {
                    hex.TerrainType = TerrainType.Desert;
                }

                if (
                    hex.TerrainType is TerrainType.Plains
                    && IsForest(mapValuesForest[x, y], maxForest)
                )
                {
                    hex.TerrainType = TerrainType.Forest;
                }

                if (
                    hex.TerrainType is TerrainType.Plains
                    && IsMountain(mapValuesMountain[x, y], maxMountain)
                )
                {
                    hex.TerrainType = TerrainType.Mountain;
                }

                // Generate ice on poles
                if (y < r.Next(iceDepth) + 1 || y > Height - (r.Next(iceDepth) + 1))
                {
                    hex.TerrainType = TerrainType.Ice;
                }

                BaseLayer.SetCell(coords, 0, terrainToTextureCoords[hex.TerrainType]);
                BorderLayer.SetCell(coords, 0, Vector2I.Zero);

                switch (hex.TerrainType)
                {
                    case TerrainType.Plains:
                        hex.Food = r.Next(2, 6);
                        hex.Production = r.Next(0, 2);
                        break;
                    case TerrainType.Forest:
                        hex.Food += r.Next(1, 3);
                        hex.Production += r.Next(2, 6);
                        break;
                    case TerrainType.Desert:
                        hex.Food = r.Next(0, 1);
                        hex.Production = r.Next(3, 6);
                        break;
                    case TerrainType.Mountain:
                        hex.Production = r.Next(4, 7);
                        break;
                    default:
                        break;
                }
                mapData[coords] = hex;
            }
        }

        Callable.From(() => GlobalEvents.RaiseMapGenerationCompleted(this)).CallDeferred();
        stopwatch.Stop();
        GD.Print($"Terrain cells generation took: {stopwatch.ElapsedMilliseconds} ms.");
    }

    private City? CreateCity(Civilization civ, Vector2I coords, string name)
    {
        // TODO: Add recursion to mark all surr hexes owned by the city.

        if (CityScene is null)
        {
            GD.PrintErr("City scene is not provided!");
            GetTree().Quit(1);
            return null;
        }

        var city = CityScene?.Instantiate<City>();
        if (city is not null)
        {
            city.Position = BaseLayer.MapToLocal(coords);
            city.Center = coords;
            city.CityName = name;
            city.OwnerCiv = civ;
            city.Map = this;
            mapData[coords].IsCityCenter = true;
            mapData[coords].CityOwner = city;
            AddChild(city);
            MarkAllTilesInRadiusAsOwnedByCity(city, 3);
            city.UpdateCityInfo();
        }
        return city;
    }

    public void GenerateCivStartingLocations(List<Civilization> civilizations)
    {
        var plains = mapData
            .Where(kv => kv.Value.TerrainType == TerrainType.Plains)
            .Select(kv => kv.Value)
            .ToList();
        var r = new Random();

        // TODO: Check if there is another civ near-by...
        foreach (var civ in civilizations)
        {
            var maxAttempts = 1000;
            var currentAttempt = 0;
            var foundValidLocation = false;
            var location = Vector2I.Zero;
            while (!foundValidLocation)
            {
                location = plains[r.Next(plains.Count)].Coords;
                foundValidLocation = IsLocationValid(location, civilizations);
                currentAttempt++;
                if (currentAttempt >= maxAttempts)
                {
                    GD.PrintErr("Failed to found starting location for civilization");
                    GetTree().Quit(1);
                }
            }

            var city = CreateCity(
                civ,
                location,
                $"{civ.Name}|{civ.GetAndRemoveRandomCityNameFromAvailableCityNames()}"
            );

            if (city is not null)
            {
                civ.Cities.Add(city);
            }
            else
            {
                GD.PrintErr("Wasn't able to create city at location.");
            }
        }
    }

    private bool IsLocationValid(Vector2I coords, List<Civilization> civilizations)
    {
        if ((coords.X < 3 || coords.X > Width - 3) || (coords.Y < 3 && coords.Y > Height - 3))
        {
            return false;
        }
        foreach (var civ in civilizations)
        {
            foreach (var city in civ.Cities)
            {
                var distance = coords.DistanceTo(city.Center);
                GD.Print(
                    $"Trying to find a good location for new city. Coords={coords}, Distance to '{city.CityName}': {distance}"
                );
                if (distance <= 12)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void MarkAllTilesInRadiusAsOwnedByCity(City city, int radiusInTiles)
    {
        Dictionary<int, List<Hex>> radiusToHexMap = [];
        radiusToHexMap[1] = GetSurroundingHexes(city.Center);

        for (int i = 1; i <= radiusInTiles; i++)
        {
            var hexes = radiusToHexMap[i];
            foreach (var hex in hexes)
            {
                if (hex.CityOwner != city)
                {
                    CreateHexOverlayAtTile(hex.Coords, i.ToString(), city.OwnerCiv);
                    hex.CityOwner = city;
                    city.TilesOwned.Add(hex);
                    var hexesInNextRadius = radiusToHexMap.GetValueOrDefault(i + 1);
                    if (hexesInNextRadius is null)
                    {
                        hexesInNextRadius = [];
                        radiusToHexMap[i + 1] = hexesInNextRadius;
                    }
                    hexesInNextRadius.AddRange(GetSurroundingHexes(hex.Coords));
                }
            }
        }
    }

    private void CreateHexOverlayAtTile(Vector2I coords, string text, Civilization owner)
    {
        var hexOverlay = HexOverlayScene?.Instantiate() as HexOverlay;
        if (hexOverlay == null)
        {
            GD.PrintErr("Hex Overlay scene not fould");
            GetTree().Quit(1);
        }
        hexOverlay?.Position = BaseLayer.MapToLocal(coords);
        AddChild(hexOverlay);
        hexOverlay?.UpdateLabel(text);
        hexOverlay?.UpdateColor(owner.Color);
    }

    private void UpdateCivOwnedHexes(Civilization civ)
    {
        foreach (var city in civ.Cities)
        {
            foreach (var h in city.TilesOwned)
            {
                // TODO
                GD.Print("Painting");
            }
        }
    }

    private List<Hex> GetSurroundingHexes(Vector2I center)
    {
        var result = new List<Hex>();
        foreach (var coords in BaseLayer.GetSurroundingCells(center))
        {
            var h = mapData.GetValueOrDefault(coords);
            //if (h is not null && h.CityOwner is null)
            if (h is not null)
            {
                result.Add(h);
            }
        }
        return result;
    }

    private static FastNoiseLite CreateNoise(int seed)
    {
        return new FastNoiseLite
        {
            Seed = seed,
            Frequency = 0.008f,
            FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
            FractalOctaves = 4,
            FractalLacunarity = 2.25f,
        };
    }

    private static FastNoiseLite CreateNoiseForest(int seed)
    {
        return new FastNoiseLite
        {
            Seed = seed,
            NoiseType = FastNoiseLite.NoiseTypeEnum.Cellular,
            Frequency = 0.04f,
            FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
            FractalLacunarity = 2f,
        };
    }

    private static FastNoiseLite CreateNoiseDesert(int seed)
    {
        return new FastNoiseLite
        {
            Seed = seed,
            NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth,
            Frequency = 0.015f,
            FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
            FractalLacunarity = 2f,
        };
    }

    private static FastNoiseLite CreateNoiseMountain(int seed)
    {
        return new FastNoiseLite
        {
            Seed = seed,
            NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex,
            Frequency = 0.05f,
            FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
            FractalWeightedStrength = 1,
        };
    }

    private static TerrainType GetBaseTerrain(float value, float max)
    {
        float normalized = value / max;

        return normalized switch
        {
            < 0.25f => TerrainType.Water,
            < 0.40f => TerrainType.Shallows,
            < 0.45f => TerrainType.Beach,
            _ => TerrainType.Plains,
        };
    }

    private static bool IsDesert(float value, float max)
    {
        float normalized = value / max;

        return normalized switch
        {
            > 0.70f => true,
            _ => false,
        };
    }

    private static bool IsForest(float value, float max)
    {
        float normalized = value / max;

        return normalized switch
        {
            > 0.75f => true,
            _ => false,
        };
    }

    private static bool IsMountain(float value, float max)
    {
        float normalized = value / max;

        return normalized switch
        {
            > 0.50f => true,
            _ => false,
        };
    }
}
