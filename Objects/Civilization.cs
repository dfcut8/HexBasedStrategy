using System;
using System.Collections.Generic;
using Godot;

namespace HexBasedStrategy.Objects;

public class Civilization
{
    public int Id { get; set; } = 0;
    public List<City> Cities { get; set; } = [];
    public Color Color { get; set; } = Colors.White;
    public string Name { get; set; } = String.Empty;
    public bool PlayerControlled { get; set; } = false;
    public List<string> CityNames { get; set; } = [];
}
