using Godot;

public partial class Cards : HBoxContainer
{
    [Export] private Card card1 = null;
    [Export] private Card card2 = null;
    [Export] private Card card3 = null;
    [Export] private Card card4 = null;
    [Export] private Card card5 = null;

    [Signal] public delegate void CardClickedEventHandler(Units.Type unit);

    private Texture2D bowTexture = null;
    private Texture2D horseTexture = null;
    private Texture2D spearTexture = null;
    private Texture2D swordTexture = null;
    private Texture2D staffTexture = null;

    public override void _Ready()
    {
        bowTexture   = GD.Load<Texture2D>("res://UI/Cards/Bow.png");
        horseTexture = GD.Load<Texture2D>("res://UI/Cards/Horse.png");
        spearTexture = GD.Load<Texture2D>("res://UI/Cards/Spear.png");
        swordTexture = GD.Load<Texture2D>("res://UI/Cards/Sword.png");
        staffTexture = GD.Load<Texture2D>("res://UI/Cards/Staff.png");

        card1.SetNumber(1); card1.SetSprite(bowTexture);   card1.unitType = Units.Type.Archers;
        card2.SetNumber(2); card2.SetSprite(horseTexture); card2.unitType = Units.Type.Cavalries;
        card3.SetNumber(3); card3.SetSprite(spearTexture); card3.unitType = Units.Type.Spearsmen;
        card4.SetNumber(4); card4.SetSprite(swordTexture); card4.unitType = Units.Type.Swordsmen;
        card5.SetNumber(5); card5.SetSprite(staffTexture); card5.unitType = Units.Type.Mages;

        card1.CardClicked += OnCardClicked;
        card2.CardClicked += OnCardClicked;
        card3.CardClicked += OnCardClicked;
        card4.CardClicked += OnCardClicked;
        card5.CardClicked += OnCardClicked;

        UpdateBuffs();
        SetGreyedOut(false);
    }

    private void OnCardClicked(Units.Type unit)
    {
        EmitSignal(SignalName.CardClicked, (int)unit);
    }

    public void UpdateBuffs()
    {
        card1.SetBuffCount(GetBuffCount(Units.Type.Archers));
        card2.SetBuffCount(GetBuffCount(Units.Type.Cavalries));
        card3.SetBuffCount(GetBuffCount(Units.Type.Spearsmen));
        card4.SetBuffCount(GetBuffCount(Units.Type.Swordsmen));
        card5.SetBuffCount(GetBuffCount(Units.Type.Mages));
    }

    public void SetGreyedOut(bool greyed)
    {
        card1.SetGreyedOut(greyed);
        card2.SetGreyedOut(greyed);
        card3.SetGreyedOut(greyed);
        card4.SetGreyedOut(greyed);
        card5.SetGreyedOut(greyed);
    }

    private int GetBuffCount(Units.Type unitType)
    {
        UnitData data = UnitRegistry.GetData(unitType);
        if (data == null) return 0;

        int count = 0;
        Modifiers.Type weather = Modifiers.GetWeather();
        Modifiers.Type terrain = Modifiers.GetTerrain(1);

        if (weather != Modifiers.Type.NoWeather && data.buffs.Contains(weather))
            count++;
        if (terrain != Modifiers.Type.NoTerrain && data.buffs.Contains(terrain))
            count++;

        return count;
    }
}