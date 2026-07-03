using Godot;
using HexBasedStrategy.Core;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Ui;

public partial class CityTile : Control
{
    public City? City { get; set; }
    private Label? name;
    private Label? population;
    private Label? production;
    private Label? food;
    private ScrollContainer? availableUnitsContainer;

    public override void _Ready()
    {
        name = GetNode<Label>("%Name/Value");
        production = GetNode<Label>("%Production/Value");
        population = GetNode<Label>("%Population/Value");
        food = GetNode<Label>("%Food/Value");
        availableUnitsContainer = GetNode<ScrollContainer>("%AvailableUnitsContainer");
    }

    public override void _Process(double delta) { }

    public void Refresh()
    {
        if (City is null)
        {
            Visible = false;
            ProcessMode = ProcessModeEnum.Disabled;
            return;
        }

        Visible = true;
        ProcessMode = ProcessModeEnum.Always;
        name?.Text = City.CityName;
        production?.Text = City.Production.ToString();
        population?.Text = City.Population.ToString();
        food?.Text = City.Food.ToString();
    }
}
