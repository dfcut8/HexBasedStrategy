using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy.Ui;

public partial class UiManager : Node2D
{
    private TerrainTile? terrainTile;
    private CityTile? cityTile;

    public override void _Ready()
    {
        terrainTile = GetNode<TerrainTile>("%TerrainTile");
        terrainTile.Visible = false;

        cityTile = GetNode<CityTile>("%CityTile");
        cityTile.Visible = false;

        GlobalEvents.HexSelected += OnHexSelected;
    }

    public override void _Process(double delta) { }

    private void DisableAllTiles()
    {
        terrainTile?.Visible = false;
        cityTile?.Visible = false;
    }

    private void OnHexSelected(Hex? h)
    {
        if (h is null)
        {
            DisableAllTiles();
            return;
        }

        if (h.IsCityCenter)
        {
            terrainTile?.Visible = false;
            cityTile?.Update(h.CityOwner);
        }
        else
        {
            cityTile?.Visible = false;
            terrainTile?.Update(h);
        }
    }
}
