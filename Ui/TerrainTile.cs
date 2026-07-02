using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy.Ui;

public partial class TerrainTile : Control
{
    public Hex? Hex { get; set; }
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

    public void Refresh()
    {
        if (Hex is null)
        {
            Visible = false;
            ProcessMode = ProcessModeEnum.Disabled;
            return;
        }
        Visible = true;
        ProcessMode = ProcessModeEnum.Always;
        terrainImage?.Texture = terrainTypeToTexture?[Hex.TerrainType].Texture;
        terrainLabel?.Text = Hex.TerrainType.ToString();
        foodLabel?.Text = Hex.Food.ToString();
        productionLabel?.Text = Hex.Production.ToString();
    }
}
