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
        var noise = CreateNoise();
        var values = new float[Width, Height];

        float max = 0f;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float value = Math.Abs(noise.GetNoise2D(x, y));
                values[x, y] = value;
                max = Math.Max(max, value);
            }
        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var coords = new Vector2I(x, y);
                var terrain = GetBaseTerrain(values[x, y], max);

                mapData[coords] = new Hex(coords) { TerrainType = terrain };

                BaseLayer.SetCell(coords, 0, terrainToTextureCoords[terrain]);
                BorderLayer.SetCell(coords, 0, Vector2I.Zero);
            }
        }

        Callable.From(() => GlobalEvents.MapGenerationCompleted?.Invoke(this)).CallDeferred();
    }

    private static FastNoiseLite CreateNoise()
    {
        return new FastNoiseLite
        {
            Seed = Random.Shared.Next(100_000),
            Frequency = 0.008f,
            FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
            FractalOctaves = 4,
            FractalLacunarity = 2.25f,
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
}
