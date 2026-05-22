using Godot;

namespace HexBasedStrategy;

public interface IHexTileMap
{
    public int Width { get; set; }
    public int Height { get; set; }

    public bool Enabled { get; set; }
    public Vector2 MapToLocal(Vector2I coords);
}
