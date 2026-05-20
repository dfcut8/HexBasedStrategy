using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy;

public partial class HexTileMap : Node2D
{
    [Export]
    public int Width = 20;

    [Export]
    public int Height = 20;

    private TileMapLayer BaseLayer;
    private TileMapLayer BorderLayer;
    private TileMapLayer OverlayLayer;

    private Dictionary<Vector2I, Hex> mapData = [];
    private readonly Dictionary<TerrainType, Vector2I> terrainToTextureCoords = new()
    {
        [TerrainType.Plains] = new Vector2I(0, 0),
        [TerrainType.Desert] = new Vector2I(0, 1),
        [TerrainType.Beach] = new Vector2I(0, 2),
        [TerrainType.Ice] = new Vector2I(0, 3),
        [TerrainType.Water] = new Vector2I(1, 0),
        [TerrainType.Mountain] = new Vector2I(1, 1),
        [TerrainType.Shallows] = new Vector2I(1, 2),
        [TerrainType.Forest] = new Vector2I(1, 3),
    };

    public override void _Ready()
    {
        BaseLayer = GetNode<TileMapLayer>("BaseLayer");
        BorderLayer = GetNode<TileMapLayer>("BorderLayer");
        OverlayLayer = GetNode<TileMapLayer>("OverlayLayer");

        GenerateTerrain();
        GD.Print("HexMap Ready!");
    }

    public override void _Process(double delta) { }

    public Vector2 MapToLocal(Vector2I coords)
    {
        return BaseLayer.MapToLocal(coords);
    }

    private void GenerateTerrain()
    {
        float[,] noiseMap = new float[Width, Height];
        float[,] forestMap = new float[Width, Height];
        float[,] desertMap = new float[Width, Height];
        float[,] mountainMap = new float[Width, Height];

        var r = new Random();
        int seed = r.Next(100_000);

        // Base terrain (water, Beach, Plains)
        var noise = new FastNoiseLite
        {
            Seed = seed,
            Frequency = 0.008f,
            FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
            FractalOctaves = 4,
            FractalLacunarity = 2.25f,
        };

        var noiseMax = 0f;

        // Generator
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                noiseMap[x, y] = Math.Abs(noise.GetNoise2D(x, y));
                if (noiseMap[x, y] > noiseMax)
                {
                    noiseMax = noiseMap[x, y];
                }
            }
        }

        List<(float Min, float Max, TerrainType Type)> terrainGeneratedValues =
        [
            (0, noiseMax / 10 * 2.5f, TerrainType.Water),
            (noiseMax / 10 * 2.5f, noiseMax / 10 * 4f, TerrainType.Shallows),
            (noiseMax / 10 * 4f, noiseMax / 10 * 4.5f, TerrainType.Beach),
            (noiseMax / 10 * 4.5f, noiseMax + 0.05f, TerrainType.Plains),
        ];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Hex h = new(new Vector2I(x, y));
                float noiseValue = noiseMap[x, y];
                try
                {
                    h.TerrainType = terrainGeneratedValues
                        .First(range => noiseValue >= range.Min && noiseValue < range.Max)
                        .Type;
                    mapData[new Vector2I(x, y)] = h;
                }
                catch (Exception e)
                {
                    GD.Print(e);
                }
                BaseLayer.SetCell(new Vector2I(x, y), 0, terrainToTextureCoords[h.TerrainType]);
                //BaseLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
                BorderLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
            }
        }
        Callable
            .From(() =>
            {
                GlobalEvents.MapGenerationCompleted?.Invoke(this);
            })
            .CallDeferred();
    }
}
