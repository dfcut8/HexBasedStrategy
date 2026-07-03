using System;
using Godot;

namespace HexBasedStrategy.Objects.Units;

public partial class BaseUnit : Node2D
{
    [Export]
    public string UnitName { get; set; } = string.Empty;
}
