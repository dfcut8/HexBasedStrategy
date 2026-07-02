using System.Collections.Generic;
using Godot;
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

    private ICityGrowthSystem? cityGrowthSystem;
    public static List<Civilization> Civilizations { get; set; } = [];
    public static List<City> Cities { get; set; } = [];
    public Dictionary<Vector2I, City> coordsToCities = [];
    public int currentTurn = 1;

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
        uiManager?.UpdateUi(++currentTurn);
        cityGrowthSystem?.Process(Cities);
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
            Civilizations.Add(civ);
        }
        hexTileMap.GenerateCivStartingLocations(Civilizations);
    }
}
