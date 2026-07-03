using System;
using Godot;

namespace HexBasedStrategy.Data.Units;

[GlobalClass]
public partial class UnitData : Resource
{
    [Export]
    public required string UnitName { get; set; }

    [Export]
    public required Texture2D UnitTexture { get; set; }

    [Export]
    public required int Speed { get; set; }

    [Export]
    public required int HitPoints { get; set; }

    [Export]
    public required int AttackPower { get; set; }
}
