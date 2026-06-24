using Godot;

public partial class HexOverlay : Node2D
{
    private Label? label;
    private Sprite2D? sprite;

    public override void _Ready()
    {
        label = GetNode<Label>("%Label");
        sprite = GetNode<Sprite2D>("%Sprite");
    }

    public override void _Process(double delta) { }

    public void UpdateLabel(string text)
    {
        label?.Text = text;
    }

    public void UpdateColor(Color color)
    {
        sprite?.Modulate = new Color(color, 0.25f);
    }
}
