using System.Collections.Generic;
using HexBasedStrategy.Objects;
using HexBasedStrategy.Objects.Units;

namespace HexBasedStrategy.Core.States;

public class LevelState
{
    public List<Civilization> Civilizations { get; set; } = [];
    public List<City> Cities { get; set; } = [];

    public BaseUnit? CurrentlySelectedUnit { get; set; }
}
