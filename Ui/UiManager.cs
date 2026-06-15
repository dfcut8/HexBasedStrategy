using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy.Ui;

public partial class UiManager : Node2D
{
    private TerrainTile? terrainTile;

    public override void _Ready()
    {
        terrainTile = GetNode<TerrainTile>("%TerrainTile");
        terrainTile.Visible = false;
        GlobalEvents.HexSelected += OnHexSelected;
    }

    public override void _Process(double delta) { }

    private void OnHexSelected(Hex? h)
    {
        terrainTile?.Update(h);
    }
}
