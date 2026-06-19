using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Data;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Core;

public partial class Level : Node
{
    [Export]
    private CivilizationData[] CivilizationDataList { get; set; } = [];

    public List<Civilization> Civilizations { get; set; } = [];
    public Dictionary<Vector2I, City> coordsToCities = [];

    public override void _Ready()
    {
        foreach (var cd in CivilizationDataList)
        {
            Civilizations.Add(new Civilization() { Color = cd.Color, Name = cd.Name });
        }
    }
}
