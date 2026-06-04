using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

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
                mapValuesMountain[x, y] = valueDesert;
                maxMountain = Math.Max(maxMountain, valueMountain);
            }
        }
        stopwatch.Stop();

        GD.Print($"Terrain data generation took: {stopwatch.ElapsedMilliseconds} ms.");

        GD.Print(
            $"max={max}, maxForest={maxForest}, maxDesert={maxDesert}, maxMountain={maxMountain}"
        );

        stopwatch.Restart();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var coords = new Vector2I(x, y);
                var terrain = GetBaseTerrain(mapValues[x, y], max);

                mapData[coords] = new Hex(coords) { TerrainType = terrain };
                if (
                    mapData[coords].TerrainType is TerrainType.Plains
                    && IsDesert(mapValuesDesert[x, y], maxDesert)
                )
                {
                    mapData[coords].TerrainType = TerrainType.Desert;
                }

                if (
                    mapData[coords].TerrainType is TerrainType.Plains
                    && IsForest(mapValuesForest[x, y], maxForest)
                )
                {
                    mapData[coords].TerrainType = TerrainType.Forest;
                }

                if (
                    mapData[coords].TerrainType is TerrainType.Plains
                    && IsMountain(mapValuesMountain[x, y], maxMountain)
                )
                {
                    mapData[coords].TerrainType = TerrainType.Mountain;
                }

                BaseLayer.SetCell(coords, 0, terrainToTextureCoords[mapData[coords].TerrainType]);
                BorderLayer.SetCell(coords, 0, Vector2I.Zero);
            }
        }

        Callable.From(() => GlobalEvents.MapGenerationCompleted?.Invoke(this)).CallDeferred();
        stopwatch.Stop();
        GD.Print($"Terrain cells generation took: {stopwatch.ElapsedMilliseconds} ms.");
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
            FractalType = FastNoiseLite.FractalTypeEnum.Ridged,
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
            > 0.45f => true,
            _ => false,
        };
    }
}
