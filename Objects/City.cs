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
    public List<Hex> TilesOwned { get; private set; } = [];
    public int Population { get; set; }
    public int Production { get; set; }
    public int Food { get; set; }

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

    public void UpdateCityInfo()
    {
        foreach (var tile in TilesOwned)
        {
            Production += tile.Production;
            Food += tile.Food;
            Population++;
        }
    }
}
