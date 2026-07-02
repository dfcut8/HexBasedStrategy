using System.Collections.Generic;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Core.States;

public class LevelState
{
    public List<Civilization> Civilizations { get; set; } = [];
    public List<City> Cities { get; set; } = [];
}
