using System;
using Godot;
using HexBasedStrategy.Data.Units;
using HexBasedStrategy.Objects.Units;

namespace HexBasedStrategy.Ui;

public partial class UnitBuildButton : TextureButton
{
    public UnitData? UnitData;

    // public required BaseUnit u;
    public Action<UnitData>? UnitButtonPressed;

    public override void _Ready()
    {
        if (UnitData is null)
        {
            GD.PrintErr("Unit Data can't be null");
            GetTree().Quit(1);
        }
        TextureNormal = UnitData?.UnitTexture;
        Pressed += OnPressed;
    }

    private void OnPressed()
    {
        if (UnitData is not null)
        {
            GD.Print($"[UnitBuildButton] Was Pressed for unit: {UnitData.UnitName}");
            UnitButtonPressed?.Invoke(UnitData);
        }
    }
}
