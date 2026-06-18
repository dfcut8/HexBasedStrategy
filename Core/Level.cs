using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Data;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Core;

public partial class Level
{
    [Export]
    private List<CivilizationData> CivilizationsDate { get; set; } = [];
    public List<Civilization> Civilizations { get; set; } = [];
    public Dictionary<Vector2I, City> coordsToCities = [];

    public void CreateCivilizations() { }
}
