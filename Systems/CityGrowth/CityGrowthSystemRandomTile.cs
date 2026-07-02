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
            if (availableTiles.Count <= 0)
            {
                GD.PrintErr("No way to expand city");
                continue;
            }
            var selected = availableTiles[Random.Shared.Next(availableTiles.Count - 1)];
            selected.CityOwner = c;
            c.TilesOwned.Add(selected);
            c.UpdateState();
            GD.Print($"Selected hex for expansion: {selected}");
        }
    }
}
