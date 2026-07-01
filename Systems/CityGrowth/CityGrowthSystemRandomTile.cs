using System;
using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Systems.CityGrowth;

public partial class CityGrowthSystemRandomTile : ICityGrowthSystem
{
    public CityGrowthSystemRandomTile()
    {
        GD.Print("CityGrowthSystemRandomTile created");
    }

    public void Process(List<City> cities)
    {
        // TODO: Get list of all cities and add a new tile
        foreach (var c in cities)
        {
            var x = c.TilesAvailableForOwnership;
        }
    }
}
