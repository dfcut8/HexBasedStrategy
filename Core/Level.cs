using System;
using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Core.States;
using HexBasedStrategy.Data;
using HexBasedStrategy.Data.Units;
using HexBasedStrategy.Objects;
using HexBasedStrategy.Objects.Units;
using HexBasedStrategy.Systems.CityGrowth;
using HexBasedStrategy.Ui;

namespace HexBasedStrategy.Core;

public partial class Level : Node
{
    [Export]
    private CivilizationData[] CivilizationDataList { get; set; } = [];

    [Export]
    private UnitData[] BaseUnitsDataList { get; set; } = [];

    [Export]
    private CityGrowthSystemType cityGrowthType;
    public Dictionary<Vector2I, City> coordsToCities = [];
    public int currentTurn = 1;

    private LevelState State { get; } = new();
    private ICityGrowthSystem? cityGrowthSystem;
    private UiManager? uiManager;
    private HexTileMap? hexTileMap;

    public override void _Ready()
    {
        hexTileMap = GetNode<HexTileMap>("%HexTileMap");
        CreateCivilizations(hexTileMap);

        uiManager = GetNode<UiManager>("%UiManager");
        uiManager.UpdateUi(currentTurn);

        cityGrowthSystem = ICityGrowthSystem.GetInstance(cityGrowthType);

        GlobalEvents.EndTurnButtonPressed += OnEndTurnButtonPressed;
        GlobalEvents.UnitSelected += OnUnitSelected;
    }

    private void OnUnitSelected(BaseUnit unit)
    {
        if (LevelState.CurrentlySelectedUnit is not null && unit == State.CurrentlySelectedUnit)
        {
            return;
        }
        if (State.CurrentlySelectedUnit is null)
        {
            State.CurrentlySelectedUnit = unit;
            State.CurrentlySelectedUnit.Select();
        }
        if (State.CurrentlySelectedUnit is not null && unit != State.CurrentlySelectedUnit)
        {
            State.CurrentlySelectedUnit.Deselect();
            State.CurrentlySelectedUnit = unit;
            State.CurrentlySelectedUnit.Select();
        }
        uiManager?.RefreshUnitTile(unit);
    }

    private void OnEndTurnButtonPressed()
    {
        cityGrowthSystem?.Process(State.Cities);
        hexTileMap?.UpdateCities(State.Cities);
        uiManager?.UpdateUi(++currentTurn);
        uiManager?.RefreshUnitTile(State?.CurrentlySelectedUnit);
    }

    private void CreateCivilizations(HexTileMap hexTileMap)
    {
        foreach (var cd in CivilizationDataList)
        {
            var civ = new Civilization()
            {
                Color = cd.Color,
                Name = cd.Name,
                CityNames = [.. cd.CityNames],
                AvailableUnits = [.. BaseUnitsDataList],
            };
            State.Civilizations.Add(civ);
        }
        State.Cities = hexTileMap.GenerateCivStartingLocations(State.Civilizations);
    }
}
