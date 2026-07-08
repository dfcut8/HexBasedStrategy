using System;
using Godot;
using HexBasedStrategy.Core;
using HexBasedStrategy.Data.Units;

namespace HexBasedStrategy.Objects.Units;

public partial class BaseUnit : Node2D
{
    [Export]
    public required UnitData Data { get; set; }

    public Vector2I Coords { get; set; } = Vector2I.Zero;
    public required Civilization CivOwner { get; set; }

    private Area2D? area;
    private Sprite2D? sprite;
    private bool isSelected;

    public override void _Ready()
    {
        if (Data is null)
        {
            GD.PrintErr("Unit data should be attached to unit");
            GetTree().Quit(1);
            return;
        }
        sprite = GetNode<Sprite2D>("Sprite2D");
        sprite.Texture = Data.UnitTexture;
        sprite.Modulate = new Color(CivOwner.Color);
        Name = Data.UnitName;
        area = GetNode<Area2D>("%Area2D");
        area.InputEvent += OnAreaClick;
    }

    private void OnAreaClick(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouse)
        {
            if (mouse.ButtonMask == MouseButtonMask.Left)
            {
                GD.Print($"Unit must be selected");
                GlobalEvents.RaiseUnitSelected(this);
            }
        }
    }

    public void Select()
    {
        isSelected = true;
        var color = new Color(CivOwner.Color);
        color.V -= 0.5f;
        sprite?.Modulate = color;
    }

    internal void Deselect()
    {
        isSelected = false;
        sprite?.Modulate = CivOwner.Color;
    }
}
