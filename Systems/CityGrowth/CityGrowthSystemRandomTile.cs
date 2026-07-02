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
        foreach (var c in cities)
        {
            var availableTiles = c.TilesAvailableForOwnership;
            var selected = availableTiles[Random.Shared.Next(availableTiles.Count)];
            c.TilesOwned.Add(selected);
            c.Update();
        }
    }
}
