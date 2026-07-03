using System;
using Godot;

namespace HexBasedStrategy.Data.Units;

[GlobalClass]
public partial class UnitData : Resource
{
    public required string UnitName { get; set; }
    public required Texture2D UnitTexture { get; set; }
    public required int Speed { get; set; }
    public required int HitPoints { get; set; }
    public required int AttackPower { get; set; }
}
