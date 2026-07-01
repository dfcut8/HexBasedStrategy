using System;
using Godot;

namespace HexBasedStrategy.Systems.CityGrowth;

public partial class CityGrowthSystemRandomTile : ICityGrowthSystem
{
    public CityGrowthSystemRandomTile()
    {
        GD.Print("CityGrowthSystemRandomTile created");
    }
}
