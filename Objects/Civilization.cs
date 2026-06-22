using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace HexBasedStrategy.Objects;

public class Civilization
{
    public int Id { get; set; } = 0;
    public List<City> Cities { get; set; } = [];
    public Color Color { get; set; } = Colors.White;
    public string Name { get; set; } = String.Empty;
    public bool PlayerControlled { get; set; } = false;
    public HashSet<string> CityNames { get; init; } = [];

    private HashSet<string> availableCityNames = [];

    public Civilization()
    {
        availableCityNames = CityNames;
    }

    public string GetAndRemoveRandomCityNameFromAvailableCityNames()
    {
        var r = new Random();
        var i = r.Next(availableCityNames.Count);
        var name = availableCityNames.ElementAt(i);
        availableCityNames.Remove(name);
        return name;
    }
}
