using System;

namespace HexBasedStrategy.Core;

internal static class GlobalEvents
{
    public static event Action<HexTileMap>? MapGenerationCompleted;
    public static event Action<Hex?>? HexSelected;

    public static void RaiseMapGenerationCompleted(HexTileMap map)
    {
        MapGenerationCompleted?.Invoke(map);
    }

    public static void RaiseHexSelected(Hex? h)
    {
        HexSelected?.Invoke(h);
    }
}
