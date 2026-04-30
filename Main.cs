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
    [Export] private DayNight dayNight = null;
    [Export] private Terrain terrainPlayer = null;
    [Export] private Terrain terrainEnemy = null;
    [Export] private Weather weather = null;
    [Export] private Cards cards = null;
    [Export] private AudioStreamPlayer bgm = null;

    private Unit p1Unit = null;
    private Unit p2Unit = null;
    
    private float waitClock = 1.5f;
    private float waitTimer = 0;
    private bool roundActive = false;
    private bool planetTransitioning = false;

    private Units.Type pendingPlayer1Unit = Units.Type.None;
    private Units.Type pendingPlayer2Unit = Units.Type.None;
    private bool damageApplied = false;
    private bool gameOver = false;

    int round = 1;
    
    public override void _Ready()
    {
        player1 = playerScene.Instantiate<Player>();
        player2 = aiScene.Instantiate<AI>();
        
        player1.SetDisplayName("Player");
        player2.SetDisplayName("Enemy");

        player1.playerIndex = 1;
        player2.playerIndex = 2;
        
        player1.healthBar.GlobalPosition = p3.GlobalPosition;
        player2.healthBar.GlobalPosition = p4.GlobalPosition;
        
        AddChild(player1);
        AddChild(player2);

        terrainPlayer.UpdateTerrain();
        terrainEnemy.UpdateTerrain();
        weather.UpdateWeather();
        cards.UpdateBuffs();
        cards.SetGreyedOut(false);
        cards.CardClicked += OnCardClicked;
    }

    private void OnCardClicked(Units.Type unit)
    {
        if (player1 is Player humanPlayer)
            humanPlayer.SelectUnit(unit);
    }

    private void CheckGameOver()
    {
        if (player1.GetCurrentHealth() <= 0)
        {
            gameOver = true;
            if (bgm != null) bgm.Stop();
            GetTree().ChangeSceneToFile("res://EndGame/Defeat.tscn");
        }
        else if (player2.GetCurrentHealth() <= 0)
        {
            gameOver = true;
            if (bgm != null) bgm.Stop();
            GetTree().ChangeSceneToFile("res://EndGame/Victory.tscn");
        }
    }

    public override void _Process(double delta)
    {
        if (gameOver)
            return;
            
        float halfWait = waitClock / 2f;

        if (roundActive)
        {
            if (waitTimer < waitClock)
            {
                waitTimer += (float)delta;

                if (!damageApplied && waitTimer >= halfWait)
                {
                    player1.TakeDamageFrom(pendingPlayer2Unit);
                    player2.TakeDamageFrom(pendingPlayer1Unit);
                    damageApplied = true;

                    player1.ResetChosenUnit();
                    player2.ResetChosenUnit();

                    cards.SetGreyedOut(true);
                    CheckGameOver();
                }

                return;
            }

            if (p1Unit != null) { p1Unit.QueueFree(); p1Unit = null; }
            if (p2Unit != null) { p2Unit.QueueFree(); p2Unit = null; }

            roundActive = false;
            planetTransitioning = true;
            dayNight.Transition();
            return;
        }

        if (planetTransitioning)
        {
            if (dayNight.IsTransitioning())
                return;

            planetTransitioning = false;

            Modifiers.AdvanceDayNight();
            Modifiers.RollWeather();
            Modifiers.RollTerrain();

            terrainPlayer.UpdateTerrain();
            terrainEnemy.UpdateTerrain();
            weather.UpdateWeather();

            cards.UpdateBuffs();
            cards.SetGreyedOut(false);
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

        if (player1UnitData.AttackScene != null)
        {
            Unit attackInstance = player1UnitData.AttackScene.Instantiate<Unit>();
            p1Unit.SetAttack(attackInstance.Texture, halfWait);
            attackInstance.Free();
        }
        if (player2UnitData.AttackScene != null)
        {
            Unit attackInstance = player2UnitData.AttackScene.Instantiate<Unit>();
            p2Unit.SetAttack(attackInstance.Texture, halfWait);
            attackInstance.Free();
        }

        pendingPlayer1Unit = player1Unit;
        pendingPlayer2Unit = player2Unit;
        damageApplied = false;

        waitTimer = 0;
        roundActive = true;
        round++;

        player2.aiMemory.AddMove(player1Unit);
        
        player1.isReady = false;
        player2.isReady = false;
        
        Debug.Print(" ");
    }
}