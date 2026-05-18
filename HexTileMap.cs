using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy;

public partial class HexTileMap : Node2D
{
    [Export]
    public int Width = 100;

    [Export]
    public int Height = 60;

    public TileMapLayer BaseLayer;
    public TileMapLayer BorderLayer;
    public TileMapLayer OverlayLayer;

    public override void _Ready()
    {
        BaseLayer = GetNode<TileMapLayer>("BaseLayer");
        BorderLayer = GetNode<TileMapLayer>("BorderLayer");
        OverlayLayer = GetNode<TileMapLayer>("OverlayLayer");

        GenerateTerrain();
        GD.Print("HexMap Ready!");
    }

    public override void _Process(double delta) { }

    public void GenerateTerrain()
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
                GlobalEvents.MapGenerationCompleted?.Invoke();
            })
            .CallDeferred();
    }
}
