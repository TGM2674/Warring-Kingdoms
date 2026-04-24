using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Player : UnitController
{
    public override void ProcessTurn(double delta)
    {
        if (Input.IsActionJustPressed("1"))
        {
            chosenUnit = Units.Type.Archers;
            Debug.Print(name + " Chose:");
            Debug.Print(chosenUnit.ToString());
            isReady = true;
        }
        if (Input.IsActionJustPressed("2"))
        {
            chosenUnit = Units.Type.Cavalries;
            Debug.Print(name + " Chose:");
            Debug.Print(chosenUnit.ToString());
            isReady = true;
        }
        if (Input.IsActionJustPressed("3"))
        {
            chosenUnit = Units.Type.Spearsmen;
            Debug.Print(name + " Chose:");
            Debug.Print(chosenUnit.ToString());
            isReady = true;
        }
    }
}