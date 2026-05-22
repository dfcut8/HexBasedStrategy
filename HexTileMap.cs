using System;
using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy;

public partial class HexTileMap : Node2D
{
    [Export]
    public int Width = 20;

    [Export]
    public int Height = 20;

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

    private record MapGenerationResult(float Max, float[,] GeneratedData);

    private MapGenerationResult GenerateMapData();

    private void GenerateTerrain()
    {
        var r = new Random();
        var noise = CreateNoise(r.Next(maxSeed));
        var noiseForest = CreateNoiseForest(r.Next(maxSeed));
        var noiseDesert = CreateNoiseDesert(r.Next(maxSeed));
        var mapValues = new float[Width, Height];
        var mapForestValues = new float[Width, Height];
        var mapDesertValues = new float[Width, Height];

        float max = 0f;
        float maxForest = 0f;
        float maxDesert = 0f;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float value = Math.Abs(noise.GetNoise2D(x, y));
                mapValues[x, y] = value;
                max = Math.Max(max, value);

                float valueForest = Math.Abs(noiseForest.GetNoise2D(x, y));
                mapForestValues[x, y] = valueForest;
                maxForest = Math.Max(maxForest, valueForest);

                float valueDesert = Math.Abs(noiseDesert.GetNoise2D(x, y));
                mapDesertValues[x, y] = valueDesert;
                maxDesert = Math.Max(maxDesert, valueDesert);
            }
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var coords = new Vector2I(x, y);
                var terrain = GetBaseTerrain(mapValues[x, y], max);

                mapData[coords] = new Hex(coords) { TerrainType = terrain };
                if (
                    mapData[coords].TerrainType is TerrainType.Plains
                    && IsDesert(mapDesertValues[x, y], maxDesert)
                )
                {
                    mapData[coords].TerrainType = TerrainType.Desert;
                }

                if (
                    mapData[coords].TerrainType is TerrainType.Plains
                    && IsForest(mapForestValues[x, y], maxForest)
                )
                {
                    mapData[coords].TerrainType = TerrainType.Forest;
                }

                BaseLayer.SetCell(coords, 0, terrainToTextureCoords[mapData[coords].TerrainType]);
                BorderLayer.SetCell(coords, 0, Vector2I.Zero);
            }
        }

        Callable.From(() => GlobalEvents.MapGenerationCompleted?.Invoke(this)).CallDeferred();
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
}
