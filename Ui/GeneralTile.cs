using System;
using Godot;
using HexBasedStrategy.Core;

namespace HexBasedStrategy.Ui;

public partial class GeneralTile : Control
{
    private Label? turnLabel;
    private Button? endTurnBtn;

    public override void _Ready()
    {
        turnLabel = GetNode<Label>("%TurnValue");
        endTurnBtn = GetNode<Button>("%EndTurn");
        endTurnBtn.Pressed += OnEndTurnBtnPressed;
    }

    private void OnEndTurnBtnPressed()
    {
        GlobalEvents.RaiseEndTurnButtonPressed();
    }

    public void Update(int currentTurn)
    {
        turnLabel?.Text = currentTurn.ToString();
    }

    public override void _Process(double delta) { }
}
