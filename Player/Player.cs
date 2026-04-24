using Godot;
using System.Diagnostics;

public partial class Player : UnitController
{
    public override void ProcessTurn(double delta)
    {
        if (isReady)
            return;

        if (Input.IsActionJustPressed("1"))
        {
            chosenUnit = Units.Type.Archers;
            Debug.Print(name + " Chose: " + chosenUnit);
            isReady = true;
        }
        else if (Input.IsActionJustPressed("2"))
        {
            chosenUnit = Units.Type.Cavalries;
            Debug.Print(name + " Chose: " + chosenUnit);
            isReady = true;
        }
        else if (Input.IsActionJustPressed("3"))
        {
            chosenUnit = Units.Type.Spearsmen;
            Debug.Print(name + " Chose: " + chosenUnit);
            isReady = true;
        }
        else if (Input.IsActionJustPressed("4"))
        {
            chosenUnit = Units.Type.Swordsmen;
            Debug.Print(name + " Chose: " + chosenUnit);
            isReady = true;
        }
        else if (Input.IsActionJustPressed("5"))
        {
            chosenUnit = Units.Type.Mages;
            Debug.Print(name + " Chose: " + chosenUnit);
            isReady = true;
        }
    }
}