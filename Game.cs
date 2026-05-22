using System.Linq;
using Godot;

namespace HexBasedStrategy;

public partial class Game : Node
{
    [Export]
    public Node2D ActiveTileMap { get; set; }

    public override void _Ready()
    {
        var tileMaps = GetChildren().OfType<IHexTileMap>().ToList();
        foreach (var tileMap in tileMaps)
        {
            if (tileMap == ActiveTileMap)
            {
                tileMap.Enabled = false;
            }
        }
        GD.Print("Game Ready!");
    }
}
