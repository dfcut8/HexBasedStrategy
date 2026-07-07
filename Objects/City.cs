using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HexBasedStrategy.Core;
using HexBasedStrategy.Data.Units;
using HexBasedStrategy.Objects.Units;

namespace HexBasedStrategy.Objects;

public partial class City : Node2D
{
    [Export]
    public PackedScene? BaseUnitScene;

    public required string CityName { get; set; }
    public required Civilization OwnerCiv { get; set; }
    public required HexTileMap WorldMap { private get; set; }
    public Vector2I Center { get; set; } = Vector2I.Zero;
    public List<Hex> TilesOwned { get; set; } = [];
    public List<Hex> TilesAvailableForOwnership { get; set; } = [];
    public int Population { get; set; }
    public int Production { get; set; }
    public int Food { get; set; }
    public int HarvestedFood { get; set; }

    // Build queue
    public Queue<UnitData> BuildQueue { get; set; } = [];
    public UnitData? BuildCurrent { get; set; }
    public int ProductionTracker { get; set; }

    private Label? label;
    private Sprite2D? sprite;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        label.Text = CityName;
        sprite = GetNode<Sprite2D>("Sprite");
        sprite.Modulate = OwnerCiv.Color;
    }

    public override void _Process(double delta) { }

    public override string ToString()
    {
        return $"[City] CityName={CityName}, OwnerCiv={OwnerCiv}, Center={Center}, TilesOwned={TilesOwned.Count}, TilesAvailableForOwnership={TilesAvailableForOwnership.Count}, HarvestedFood={HarvestedFood}";
    }

    public void UpdateState()
    {
        Production = 0;
        Food = 0;
        foreach (var tile in TilesOwned)
        {
            Production += tile.Production;
            Food += tile.Food;
            Population = TilesOwned.Count;
        }
        HarvestedFood += Food;
        if (BuildCurrent is not null)
        {
            ProductionTracker += Production;
        }
        UpdateTilesAvailableForOwnership();
        UpdateCurrentBuildingUnitInQueue();
    }

    private void UpdateCurrentBuildingUnitInQueue()
    {
        if (BuildCurrent?.Cost <= ProductionTracker)
        {
            // Means city completed the current construction.
            // At this moment we will just reset production to avoid any issues.
            SpawnUnit(BuildCurrent);
            if (BuildQueue.Count <= 1)
            {
                ProductionTracker = 0;
                BuildCurrent = null;
                BuildQueue.Dequeue();
            }
            else
            {
                ProductionTracker -= BuildCurrent.Cost;
                BuildCurrent = BuildQueue.Dequeue();
            }
        }
    }

    private void SpawnUnit(UnitData data)
    {
        var instance = BaseUnitScene?.Instantiate<BaseUnit>();
        if (instance is not null)
        {
            instance.Coords = Center;
            instance.CivOwner = OwnerCiv;
            instance.Data = data;
            AddChild(instance);
        }
    }

    private void UpdateTilesAvailableForOwnership()
    {
        var tilesOwned = TilesOwned;
        var tilesAvailableForOwnership = new List<Hex>();
        foreach (var tile in tilesOwned)
        {
            tilesAvailableForOwnership.AddRange(
                WorldMap
                    .GetSurroundingTiles(tile.Coords)
                    .ToList()
                    .Where(t =>
                        t.CityOwner is null
                        && !tilesAvailableForOwnership.Contains(t)
                        && (
                            t.TerrainType == TerrainType.Plains
                            || t.TerrainType == TerrainType.Desert
                            || t.TerrainType == TerrainType.Forest
                        )
                    )
            );
        }
        TilesAvailableForOwnership = tilesAvailableForOwnership;
    }

    public bool AddToBuildQueue(UnitData data)
    {
        var success = false;
        if (BuildCurrent is null)
        {
            BuildCurrent = data;
        }
        if (BuildQueue.Count < GlobalConstants.CityBuildQueueMaxSize)
        {
            BuildQueue.Enqueue(data);
            success = true;
        }
        return success;
    }
}
