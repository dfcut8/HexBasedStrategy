using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy.Ui;

public partial class UiManager : Node2D
{
    private TerrainTile? terrainTile;
    private CityTile? cityTile;
    private GeneralTile? generalTile;

    public override void _Ready()
    {
        terrainTile = GetNode<TerrainTile>("%TerrainTile");
        terrainTile.Visible = false;

        cityTile = GetNode<CityTile>("%CityTile");
        cityTile.Visible = false;

        generalTile = GetNode<GeneralTile>("%GeneralTile");

        GlobalEvents.HexSelected += OnHexSelected;
    }

    public override void _Process(double delta) { }

    private void DisableAllTiles()
    {
        terrainTile?.Visible = false;
        cityTile?.Visible = false;
    }

    public void UpdateUi(int currentTurn)
    {
        generalTile?.Refresh(currentTurn);
        cityTile?.Refresh();
        terrainTile?.Refresh();
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
            terrainTile?.Hex = null;
            cityTile?.City = h.CityOwner;
            cityTile?.Refresh();
        }
        else
        {
            cityTile?.Visible = false;
            cityTile?.City = null;
            terrainTile?.Hex = h;
            terrainTile?.Refresh();
        }
    }
}
