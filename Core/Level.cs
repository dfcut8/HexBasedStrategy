using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Data;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Core;

public partial class Level : Node
{
    [Export]
    private CivilizationData[] CivilizationDataList { get; set; } = [];

    public List<Civilization> Civilizations { get; set; } = [];
    public Dictionary<Vector2I, City> coordsToCities = [];

    private HexTileMap? hexTileMap;

    public override void _Ready()
    {
        hexTileMap = GetNode<HexTileMap>("%HexTileMap");
        CreateCivilizations(hexTileMap);
    }

    private void CreateCivilizations(HexTileMap hexTileMap)
    {
        foreach (var cd in CivilizationDataList)
        {
            var civ = new Civilization() { Color = cd.Color, Name = cd.Name };
            Civilizations.Add(civ);
        }
        hexTileMap.GenerateCivStartingLocations(Civilizations);
    }
}
