using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy.Ui;

public partial class TerrainTile : Control
{
    private Hex? h;
    private TextureRect? terrainImage;
    private Label? terrainLabel;
    private Label? foodLabel;
    private Label? productionLabel;

    public override void _Ready()
    {
        terrainImage = GetNode<TextureRect>("%TerrainImage");
        terrainLabel = GetNode<Label>("%TerrainLabel");
        foodLabel = GetNode<Label>("%FoodLabel");
        productionLabel = GetNode<Label>("%ProductionLabel");
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
        terrainLabel?.Text = h.TerrainType.ToString();
        foodLabel?.Text = h.Food.ToString();
        productionLabel?.Text = h.Production.ToString();
    }
}
