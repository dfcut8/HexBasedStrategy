using System;
using Godot;
using HexBasedStrategy.Objects.Units;

namespace HexBasedStrategy.Ui;

public partial class UnitTile : Control
{
    public BaseUnit? Unit;
    private Label? name;
    private Label? cost;
    private Label? speed;
    private Label? hitPoints;
    private Label? attackPower;

    public override void _Ready()
    {
        name = GetNode<Label>("%Name/Value");
        cost = GetNode<Label>("%Cost/Value");
        speed = GetNode<Label>("%Speed/Value");
        hitPoints = GetNode<Label>("%HitPoints/Value");
        attackPower = GetNode<Label>("%AttackPower/Value");
        Refresh();
    }

    public void Refresh()
    {
        name?.Text = Unit?.Data.UnitName;
        cost?.Text = Unit?.Data.Cost.ToString();
        speed?.Text = Unit?.Data.Speed.ToString();
        hitPoints?.Text = Unit?.Data.HitPoints.ToString();
        attackPower?.Text = Unit?.Data.AttackPower.ToString();
    }
}
