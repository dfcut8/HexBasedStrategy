using Godot;

public partial class HexOverlay : Node2D
{
    private Label? label;

    public override void _Ready()
    {
        label = GetNode<Label>("%Label");
    }

    public override void _Process(double delta) { }

    public void UpdateLabel(string text)
    {
        label?.Text = text;
    }
}
