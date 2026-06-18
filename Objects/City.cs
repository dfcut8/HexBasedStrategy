using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy.Objects;

public partial class City : Node2D
{
    public required string CityName { get; set; }
    public required Civilization OwnerCiv { get; set; }
    public HexTileMap? Map { private get; set; }
    public Vector2I Center { get; set; } = Vector2I.Zero;

    public List<Hex> Territory { get; private set; } = [];
    public List<Hex> TerritoryPool { get; private set; } = [];

    private Label? label;
    private Sprite2D? sprite;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        label.Text = CityName;
        sprite = GetNode<Sprite2D>("Sprite");
        sprite.Modulate = OwnerCiv.Color;
    }

    public override void _Process(double delta) { }

    public void PopulateTerritory(List<Hex> hexesToAdd)
    {
        foreach (var hex in hexesToAdd)
        {
            hex.CityOwner = this;
        }
        Territory.AddRange(hexesToAdd);
    }
}
