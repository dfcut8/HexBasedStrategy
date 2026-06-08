using Godot;

namespace HexBasedStrategy.Core;

public class Hex(Vector2I coords)
{
    public Vector2I Coords { get; } = coords;
    public TerrainType TerrainType { get; set; }

    public override string ToString()
    {
        return $"Hex: TerrainType={TerrainType}, Coords={Coords}";
    }
}
