using System.Collections.Generic;
using HexBasedStrategy.Core;

namespace HexBasedStrategy.Ui;

internal class TerrainTypeLoader
{
    public static Dictionary<TerrainType, TerrainTypeTexture> LoadTerrainTypes()
    {
        Dictionary<TerrainType, TerrainTypeTexture> output = [];
        output.Add(
            TerrainType.Plains,
            new TerrainTypeTexture("Plains", "res://Assets/Textures/TerrainTypes/Plains.jpg")
        );
        output.Add(
            TerrainType.Desert,
            new TerrainTypeTexture("Desert", "res://Assets/Textures/TerrainTypes/Desert.jpg")
        );
        output.Add(
            TerrainType.Beach,
            new TerrainTypeTexture("Beach", "res://Assets/Textures/TerrainTypes/Beach.jpg")
        );
        output.Add(
            TerrainType.Forest,
            new TerrainTypeTexture("Forest", "res://Assets/Textures/TerrainTypes/Forest.jpg")
        );
        output.Add(
            TerrainType.Ice,
            new TerrainTypeTexture("Ice", "res://Assets/Textures/TerrainTypes/Ice.jpg")
        );
        output.Add(
            TerrainType.Mountain,
            new TerrainTypeTexture("Mountain", "res://Assets/Textures/TerrainTypes/Mountain.jpg")
        );
        output.Add(
            TerrainType.Water,
            new TerrainTypeTexture("Ocean", "res://Assets/Textures/TerrainTypes/Ocean.jpg")
        );
        output.Add(
            TerrainType.Shallows,
            new TerrainTypeTexture("Shallow", "res://Assets/Textures/TerrainTypes/Shallow.jpg")
        );

        foreach (var r in output.Values)
        {
            r.LoadTexture();
        }
        return output;
    }
}
