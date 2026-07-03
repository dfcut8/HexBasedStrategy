using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HexBasedStrategy.Data.Units;

namespace HexBasedStrategy.Objects;

public class Civilization
{
    public int Id { get; set; } = 0;
    public List<City> Cities { get; set; } = [];
    public Color Color { get; set; } = Colors.White;
    public string Name { get; set; } = String.Empty;
    public bool PlayerControlled { get; set; } = false;
    public List<UnitData> AvailableUnits { get; set; } = [];
    public HashSet<string> CityNames
    {
        get;
        init
        {
            field = value;
            availableCityNames = [.. value];
        }
    } = [];

    private HashSet<string> availableCityNames = [];

    public string GetAndRemoveRandomCityNameFromAvailableCityNames()
    {
        var i = Random.Shared.Next(availableCityNames.Count);
        var name = availableCityNames.ElementAt(i);
        availableCityNames.Remove(name);
        return name;
    }
}
