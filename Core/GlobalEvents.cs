using System;
using HexBasedStrategy.Objects.Units;

namespace HexBasedStrategy.Core;

internal static class GlobalEvents
{
    public static event Action<HexTileMap>? MapGenerationCompleted;
    public static event Action<Hex?>? HexSelected;
    public static event Action? EndTurnButtonPressed;

    public static Action<BaseUnit>? UnitSelected;

    public static void RaiseMapGenerationCompleted(HexTileMap map)
    {
        MapGenerationCompleted?.Invoke(map);
    }

    public static void RaiseHexSelected(Hex? h)
    {
        HexSelected?.Invoke(h);
    }

    public static void RaiseEndTurnButtonPressed()
    {
        EndTurnButtonPressed?.Invoke();
    }

    public static void RaiseUnitSelected(BaseUnit u)
    {
        UnitSelected?.Invoke(u);
    }
}
