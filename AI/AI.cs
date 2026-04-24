using Godot;
using System;
using System.Diagnostics;

public partial class AI : UnitController
{
    public override void ProcessTurn(double delta)
    {
        if (isReady || chosenUnit != Units.Type.None)
            return;

        chosenUnit = aiMemory.GetBestMove();
        aiMemory.PrintBestMove();
        isReady = true;
    }
}
