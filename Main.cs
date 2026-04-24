using Godot;
using System.Diagnostics;

public partial class Main : Node
{
    [Export] private PackedScene playerScene = null;
    [Export] private PackedScene aiScene = null;
    
    private UnitController player1 = null;
    private UnitController player2 = null;

    [Export] private Marker2D p1 = null;
    [Export] private Marker2D p2 = null;
    [Export] private Marker2D p3 = null;
    [Export] private Marker2D p4 = null;
    
    private Unit p1Unit = null;
    private Unit p2Unit = null;

    private float waitClock = 0.5f;
    private float waitTimer = 0;
    
    int round = 1;
    
    public override void _Ready()
    {
        player1 = playerScene.Instantiate<Player>();
        player2 = aiScene.Instantiate<AI>();

        waitTimer = waitClock;
        
        player1.SetName("Player");
        player2.SetName("Enemy");

        player1.playerIndex = 1;
        player2.playerIndex = 2;
        
        player1.healthBar.GlobalPosition = p3.GlobalPosition;
        player2.healthBar.GlobalPosition = p4.GlobalPosition;
        
        AddChild(player1);
        AddChild(player2);
    }

    public override void _Process(double delta)
    {
        if (waitTimer < waitClock)
        {
            waitTimer += (float)delta;
            return;
        }
        if (waitTimer >= waitClock)
        {
            if (p1Unit != null)
            {
                p1Unit.QueueFree();
                p1Unit = null;
            }

            if (p2Unit != null)
            {
                p2Unit.QueueFree();
                p2Unit = null;
            }
        }
        
        if (player1 == null || player2 == null)
            return;

        if (player1.GetCurrentHealth() <= 0 || player2.GetCurrentHealth() <= 0)
            return;
        
        player1.ProcessTurn(delta);
        player2.ProcessTurn(delta);

        if (!player1.isReady || !player2.isReady)
            return;
        
        Units.Type player1Unit = player1.GetChosenUnit();
        Units.Type player2Unit = player2.GetChosenUnit();
        
        UnitData player1UnitData = UnitRegistry.GetData(player1Unit);
        UnitData player2UnitData = UnitRegistry.GetData(player2Unit);

        p1Unit = player1UnitData.UnitScene.Instantiate<Unit>();
        p2Unit = player2UnitData.UnitScene.Instantiate<Unit>();
        
        AddChild(p1Unit);
        AddChild(p2Unit);
        p1Unit.GlobalPosition = p1.GlobalPosition;
        p2Unit.GlobalPosition = p2.GlobalPosition;
        p2Unit.FlipH = true;

        waitTimer = 0;

        // Roll all modifiers for this round
        Modifiers.AdvanceDayNight();
        Modifiers.RollWeather();
        Modifiers.RollTerrain();
        
        player1.TakeDamageFrom(player2Unit);
        player2.TakeDamageFrom(player1Unit);
        
        player1.ResetChosenUnit();
        player2.ResetChosenUnit();
        
        player1.aiMemory.AddMove(player2Unit);
        player2.aiMemory.AddMove(player1Unit);
        
        player1.isReady = false;
        player2.isReady = false;
        
        Debug.Print(" ");
    }
}