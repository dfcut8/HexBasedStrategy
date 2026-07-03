using System;
using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Systems.CityGrowth;

public partial class CityGrowthSystemRandomTile : ICityGrowthSystem
{
    private const int BaseFoodNeededPerPopulation = 12;
    private const float FoodGrowthFactor = 1.4f;

    public CityGrowthSystemRandomTile()
    {
        GD.Print("CityGrowthSystemRandomTile created");
    }

    public void Process(List<City> cities)
    {
        foreach (var c in cities)
        {
            if (!DoesCityHaveEnoughFoodToGrow(c))
            {
                c.UpdateState();
                continue;
            }
            var availableTiles = c.TilesAvailableForOwnership;
            if (availableTiles.Count <= 0)
            {
                GD.PrintErr("No way to expand city");
                continue;
            }
            var selected = availableTiles[Random.Shared.Next(availableTiles.Count)];
            selected.CityOwner = c;
            c.TilesOwned.Add(selected);
            c.HarvestedFood = 0;
            c.UpdateState();
            GD.Print($"Selected hex for expansion: {selected}");
        }
    }

    private static bool DoesCityHaveEnoughFoodToGrow(City c)
    {
        var result = BaseFoodNeededPerPopulation * MathF.Pow(FoodGrowthFactor, c.Population - 1);
        GD.Print($"City requires {result} of food to grow. HarvestedFood={c.HarvestedFood}");
        return result < c.HarvestedFood;
    }
}
