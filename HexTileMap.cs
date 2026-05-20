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

    private TileMapLayer BaseLayer;
    private TileMapLayer BorderLayer;
    private TileMapLayer OverlayLayer;

    private Dictionary<Vector2I, Hex> mapData = [];
    private Dictionary<TerrainType, Vector2I> terrainToTextureCoords = new()
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
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                BaseLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
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
