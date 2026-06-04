using Godot;

namespace HexBasedStrategy.Core;

public class Hex(Vector2I coords)
{
    public Vector2I Coords { get; } = coords;
    public TerrainType TerrainType { get; set; }
}
