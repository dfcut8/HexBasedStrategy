using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy.Ui;

public partial class TerrainTile : Control
{
    private TextureRect? terrainImage;
    private Label? terrainLabel;
    private Label? foodLabel;
    private Label? productionLabel;
    private Dictionary<TerrainType, TerrainTypeTexture>? terrainTypeToTexture;

    public override void _Ready()
    {
        terrainImage = GetNode<TextureRect>("%TerrainImage");
        terrainLabel = GetNode<Label>("%TerrainLabel");
        foodLabel = GetNode<Label>("%FoodLabel");
        productionLabel = GetNode<Label>("%ProductionLabel");
        terrainTypeToTexture = TerrainTypeLoader.LoadTerrainTypes();
    }

    public override void _Process(double delta) { }

    public void Update(Hex? h)
    {
        if (h is null)
        {
            Visible = false;
            return;
        }

        Visible = true;
        terrainImage?.Texture = terrainTypeToTexture?[h.TerrainType].Texture;
        terrainLabel?.Text = h.TerrainType.ToString();
        foodLabel?.Text = h.Food.ToString();
        productionLabel?.Text = h.Production.ToString();
    }
}
