using System;

namespace HexBasedStrategy.Core;

internal static class GlobalEvents
{
    public static event Action<HexTileMap>? MapGenerationCompleted;

    public static void RaiseMapGenerationCompleted(HexTileMap map)
    {
        MapGenerationCompleted?.Invoke(map);
    }
}
