using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Godot;
using HexBasedStrategy.Core;
using HexBasedStrategy.Data.Units;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Ui;

public partial class CityTile : Control
{
    [Export]
    private PackedScene? UnitBuildButtonScene { get; set; }

    [Export]
    public PackedScene? UnitBuildProgressTile { get; set; }
    public City? City { get; set; }
    private Label? name;
    private Label? population;
    private Label? production;
    private Label? food;
    private HBoxContainer? availableUnitsContainer;
    private HBoxContainer? queueContainer;
    private List<UniBuildProgressTile> queueSlots = [];

    public override void _Ready()
    {
        name = GetNode<Label>("%Name/Value");
        production = GetNode<Label>("%Production/Value");
        population = GetNode<Label>("%Population/Value");
        food = GetNode<Label>("%Food/Value");
        availableUnitsContainer = GetNode<HBoxContainer>("%AvailableUnitsContainer");
        queueContainer = GetNode<HBoxContainer>("%QueueContainer");
        if (queueContainer is not null && UnitBuildProgressTile is not null)
        {
            PrePopulateQueueContainer(queueContainer, UnitBuildProgressTile);
        }
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
        UpdateQueueContainer(City.BuildQueue);
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

    private void PrePopulateQueueContainer(
        HBoxContainer queueContainer,
        PackedScene UnitBuildProgressTile
    )
    {
        for (var i = 0; i < GlobalConstants.CityBuildQueueMaxSize; i++)
        {
            var slot = UnitBuildProgressTile?.Instantiate<UniBuildProgressTile>();
            if (slot is not null)
            {
                slot.Visible = false;
                queueContainer.AddChild(slot);
                queueSlots.Add(slot);
            }
        }
    }

    private void UpdateQueueContainer(Queue<UnitData> units)
    {
        if (City is not null)
        {
            var slot = queueSlots[0];
            // Update first slot to current building unit if not null.
            var building = City.BuildCurrent;
            if (building is not null)
            {
                slot.unitData = building;
                slot.Visible = true;
                var value = (double)City.ProductionTracker / slot.unitData.Cost * 100;
                slot.Refresh((int)value);
            }
            else
            {
                slot.unitData = null;
                slot.Visible = false;
            }
        }
        for (var i = 1; i < GlobalConstants.CityBuildQueueMaxSize; i++)
        {
            var unit = units.ElementAtOrDefault(i);
            var slot = queueSlots[i];
            if (unit is not null)
            {
                slot.unitData = unit;
                slot.Visible = true;
                slot.Refresh(0);
            }
            else
            {
                slot.unitData = null;
                slot.Visible = false;
            }
        }
    }

    private void OnUnitButtonPressed(UnitData data)
    {
        GD.Print(
            $"Processing 'UnitButtonPressed' event: CityName={City?.CityName}, UnitName={data.UnitName}"
        );
        City?.AddToBuildQueue(data);
        Refresh();
    }
}
