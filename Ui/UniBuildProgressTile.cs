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
        Refresh(0);
    }

    public void Refresh(int value)
    {
        FillMode = (int)FillModeEnum.Clockwise;
        TextureUnder = unitData?.UnitTexture;
        TintUnder = new Color("595959");
        TextureProgress = unitData?.UnitTexture;
        Value = value;
    }
}
