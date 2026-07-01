using System.Collections.Generic;
using HexBasedStrategy.Objects;
using HexBasedStrategy.Systems.CityGrowth;

namespace HexBasedStrategy.Systems.CityGrowth;

public interface ICityGrowthSystem
{
    public void Process(List<City> cities);

    public static ICityGrowthSystem? GetInstance(CityGrowthSystemType type)
    {
        return type switch
        {
            CityGrowthSystemType.RandomTile => new CityGrowthSystemRandomTile(),
            CityGrowthSystemType.Generational => new CityGrowthSystemGenerational(),
            _ => null,
        };
    }
}
