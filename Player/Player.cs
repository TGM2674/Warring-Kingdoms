using Godot;
using System.Diagnostics;

public partial class Player : UnitController
{
    public void SelectUnit(Units.Type unit)
    {
        if (isReady)
            return;

        chosenUnit = unit;
        Debug.Print(name + " Chose: " + chosenUnit);
        isReady = true;
    }

    public override void ProcessTurn(double delta)
    {
        if (isReady)
            return;

        if (Input.IsActionJustPressed("1"))
            SelectUnit(Units.Type.Archers);
        else if (Input.IsActionJustPressed("2"))
            SelectUnit(Units.Type.Cavalries);
        else if (Input.IsActionJustPressed("3"))
            SelectUnit(Units.Type.Spearsmen);
        else if (Input.IsActionJustPressed("4"))
            SelectUnit(Units.Type.Swordsmen);
        else if (Input.IsActionJustPressed("5"))
            SelectUnit(Units.Type.Mages);
    }
}