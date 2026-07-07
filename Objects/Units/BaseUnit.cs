using System;
using Godot;
using HexBasedStrategy.Data.Units;

namespace HexBasedStrategy.Objects.Units;

public partial class BaseUnit : Node2D
{
    [Export]
    public required UnitData Data { get; set; }

    public Vector2I Coords { get; set; } = Vector2I.Zero;
    public required Civilization CivOwner { get; set; }

    private Sprite2D? sprite;

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
    }
}
