using Godot;

namespace HexBasedStrategy.Data;

[GlobalClass]
public partial class CivilizationData : Resource
{
    [Export]
    public string Name { get; set; } = string.Empty;

    [Export]
    public Color Color { get; set; } = Colors.White;

    [Export]
    public string Description { get; set; } = string.Empty;

    [Export]
    public string[] CityNames { get; set; } = [];
}
