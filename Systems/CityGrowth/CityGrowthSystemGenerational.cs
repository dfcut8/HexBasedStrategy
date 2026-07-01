using System;
using Godot;

namespace HexBasedStrategy.Systems.CityGrowth;

public class CityGrowthSystemGenerational : ICityGrowthSystem
{
    public CityGrowthSystemGenerational()
    {
        GD.Print("CityGrowthSystemGenerational created.");
    }
}
