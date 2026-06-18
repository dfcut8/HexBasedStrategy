using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy.Objects;

public partial class City : Node2D
{
    public string CityName { get; set; } = string.Empty;
    public Civilization? CityOwner { get; set; }
    public HexTileMap? Map { private get; set; }
    public Vector2I? Center { get; set; }

    private List<Hex?>? territory;
    private List<Hex?>? territoryPool;

    private Label? label;
    private Sprite2D? sprite;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        label.Text = CityName;
        sprite = GetNode<Sprite2D>("Sprite");
    }

    public override void _Process(double delta) { }
}
