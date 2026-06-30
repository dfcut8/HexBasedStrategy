using HexBasedStrategy.Systems.CityGrowth;

namespace HexBasedStrategy.Systems.CityGrowth;

public interface ICityGrowthSystem
{
    public static ICityGrowthSystem? GetInstance(CityGrowthSystemType type)
    {
        return type switch
        {
            CityGrowthSystemType.RandomTile => new CityGrowthSystemRandomTile(),
            _ => null,
        };
    }
}
