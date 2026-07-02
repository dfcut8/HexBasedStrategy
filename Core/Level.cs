using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Core.States;
using HexBasedStrategy.Data;
using HexBasedStrategy.Objects;
using HexBasedStrategy.Systems.CityGrowth;
using HexBasedStrategy.Ui;

namespace HexBasedStrategy.Core;

public partial class Level : Node
{
    [Export]
    private CivilizationData[] CivilizationDataList { get; set; } = [];

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
    }

    private void OnEndTurnButtonPressed()
    {
        cityGrowthSystem?.Process(State.Cities);
        hexTileMap?.UpdateCities(State.Cities);
        uiManager?.UpdateUi(++currentTurn);
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
            };
            State.Civilizations.Add(civ);
        }
        State.Cities = hexTileMap.GenerateCivStartingLocations(State.Civilizations);
    }
}
