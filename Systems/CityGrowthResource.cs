using System;
using Godot;
using HexBasedStrategy.Systems;

[GlobalClass]
public partial class CityGrowthResource : Resource
{
    [Export]
    private CityGrowthSystemType cityGrowthSystemType;

    public enum CityGrowthSystemType
    {
        RandomTile,
        Complex,
    }

    public ICityGrowthSystem? GetInstance()
    {
        return cityGrowthSystemType switch
        {
            CityGrowthSystemType.RandomTile => new CityGrowthSystemRandomTile(),
            _ => null,
        };
    }
}
