using System;
using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Core;
using HexBasedStrategy.Data.Units;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Ui;

public partial class CityTile : Control
{
    [Export]
    public PackedScene? UnitBuildButtonScene { get; set; }
    public City? City { get; set; }
    private Label? name;
    private Label? population;
    private Label? production;
    private Label? food;
    private HBoxContainer? availableUnitsContainer;
    private HBoxContainer? queueContainer;

    public override void _Ready()
    {
        name = GetNode<Label>("%Name/Value");
        production = GetNode<Label>("%Production/Value");
        population = GetNode<Label>("%Population/Value");
        food = GetNode<Label>("%Food/Value");
        availableUnitsContainer = GetNode<HBoxContainer>("%AvailableUnitsContainer");
        queueContainer = GetNode<HBoxContainer>("%QueueContainer");
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

        // Potential bug related to dynamic nature of available units.
        // What if we will add more units on some condition later.
        if (availableUnitsContainer?.GetChildCount() <= 0)
        {
            UpdateUnitContainer(City.OwnerCiv.AvailableUnits);
        }
        if (City?.BuildQueue.Count > 0)
        {
            UpdateUnitContainer(City.OwnerCiv.AvailableUnits);
        }
    }

    private void UpdateUnitContainer(List<UnitData> units)
    {
        if (UnitBuildButtonScene is not null)
        {
            foreach (var ud in units)
            {
                var unitButton = UnitBuildButtonScene.Instantiate<UnitBuildButton>();
                unitButton.UnitData = ud;
                availableUnitsContainer?.AddChild(unitButton);
                unitButton.UnitButtonPressed += OnUnitButtonPressed;
            }
        }
    }

    private void UpdateQueueContainer(List<UnitData> units)
    {
        foreach (var ud in units)
        {
            queueContainer.AddChild();
        }
    }

    private void OnUnitButtonPressed(UnitData data)
    {
        GD.Print(
            $"Processing 'UnitButtonPressed' event: CityName={City?.CityName}, UnitName={data.UnitName}"
        );
        City?.AddToBuildQueue(data);
    }
}
