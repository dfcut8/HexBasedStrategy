using Godot;
using HexBasedStrategy.Core;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Ui;

public partial class CityTile : Control
{
    private Label? name;
    private Label? population;
    private Label? production;
    private Label? food;

    public override void _Ready()
    {
        name = GetNode<Label>("%Name/Value");
        production = GetNode<Label>("%Production/Value");
        population = GetNode<Label>("%Population/Value");
        food = GetNode<Label>("%Food/Value");
    }

    public override void _Process(double delta) { }

    public void Update(City? city)
    {
        if (city is null)
        {
            Visible = false;
            ProcessMode = ProcessModeEnum.Disabled;
            return;
        }

        Visible = true;
        ProcessMode = ProcessModeEnum.Always;
        name?.Text = city.CityName;
        production?.Text = city.Production.ToString();
        population?.Text = city.Population.ToString();
        food?.Text = city.Food.ToString();
    }
}
