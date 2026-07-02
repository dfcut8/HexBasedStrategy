using Godot;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Core;

public class Hex(Vector2I coords)
{
    public Vector2I Coords { get; } = coords;
    public TerrainType TerrainType { get; set; }
    public int Food;
    public int Production;
    public City? CityOwner { get; set; }
    public bool IsCityCenter { get; set; }

    public override string ToString()
    {
        return $"[Hex] TerrainType={TerrainType}, Coords={Coords}, Food={Food}, Production={Production}, CityOwner={CityOwner}";
    }
}
