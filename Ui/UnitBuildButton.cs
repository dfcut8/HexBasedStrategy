using System;
using Godot;
using HexBasedStrategy.Objects.Units;

namespace HexBasedStrategy.Ui;

public partial class UnitBuildButton : TextureButton
{
    public required BaseUnit u;
    public Action<BaseUnit>? UnitButtonPressed;

    public override void _Ready()
    {
        Pressed += OnPressed;
    }

    private void OnPressed()
    {
        GD.Print($"[UnitBuildButton] Was Pressed for unit: {u.UnitName}");
        UnitButtonPressed?.Invoke(u);
    }
}
