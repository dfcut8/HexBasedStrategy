using System;
using System.Collections.Generic;
using System.Drawing;

namespace HexBasedStrategy.Objects;

public class Civilization
{
    public int Id { get; set; } = 0;
    public List<City> Cities { get; set; } = [];
    public Color Color { get; set; } = Color.White;
    public string Name { get; set; } = String.Empty;
    public bool PlayerControllered { get; set; } = false;
}
