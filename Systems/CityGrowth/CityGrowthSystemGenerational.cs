using System;
using System.Collections.Generic;
using Godot;
using HexBasedStrategy.Objects;

namespace HexBasedStrategy.Systems.CityGrowth;

public class CityGrowthSystemGenerational : ICityGrowthSystem
{
    public CityGrowthSystemGenerational()
    {
        GD.Print("CityGrowthSystemGenerational created.");
    }

    public void Process(List<City> cities)
    {
        // TODO
    }
}
