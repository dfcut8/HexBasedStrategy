using System;
using Godot;
using HexBasedStrategy.Data.Units;

namespace HexBasedStrategy.Ui;

public partial class UniBuildProgressTile : TextureProgressBar
{
    [Export]
    public required UnitData? unitData { get; set; }

    public override void _Ready()
    {
        FillMode = (int)FillModeEnum.Clockwise;
        TextureUnder = unitData?.UnitTexture;
        TintUnder = new Color("595959");
        TextureProgress = unitData?.UnitTexture;
    }

    public void Refresh(int value)
    {
        Value = value;
    }
}
