using System.Collections.Generic;
using System.Linq;
using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy.Objects;

public partial class City : Node2D
{
    public required string CityName { get; set; }
    public required Civilization OwnerCiv { get; set; }
    public required HexTileMap WorldMap { private get; set; }
    public Vector2I Center { get; set; } = Vector2I.Zero;
    public List<Hex> TilesOwned { get; set; } = [];
    public List<Hex> TilesAvailableForOwnership { get; set; } = [];
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

    public void Update()
    {
        // TODO: Need to recalculate available pool
        UpdateTilesAvailableForOwnership();
    }

    private void UpdateTilesAvailableForOwnership()
    {
        var tilesOwned = TilesOwned;
        var tilesAvailableForOwnership = new List<Hex>();
        foreach (var tile in tilesOwned)
        {
            tilesAvailableForOwnership.AddRange(
                WorldMap
                    .GetSurroundingTiles(tile.Coords)
                    .ToList()
                    .Where(t =>
                        t.CityOwner != this
                        && (
                            t.TerrainType == TerrainType.Plains
                            || t.TerrainType == TerrainType.Desert
                            || t.TerrainType == TerrainType.Forest
                        )
                    )
            );
        }
        TilesAvailableForOwnership.AddRange(tilesAvailableForOwnership);
    }
}
