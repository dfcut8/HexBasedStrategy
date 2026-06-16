using System;
using Godot;

namespace HexBasedStrategy.Ui;

internal record TerrainTypeTexture(String? name, String? resourcePath)
{
    public Texture2D? Texture;

    public void LoadTexture()
    {
        Texture = ResourceLoader.Load<Texture2D>(resourcePath);
    }
}
