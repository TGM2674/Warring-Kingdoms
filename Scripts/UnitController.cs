using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class UnitController : Node
{
    public string name = "";
    
    [Export] private Label nameLabel = null;
    [Export] public ProgressBar healthBar = null;
    
    private float maxHealth = 5000;
    private float currentHealth = 1;

    [Export] private bool isAI = false;
    public Memory aiMemory = null;

    public int playerIndex = 1;
    
    [Export] private Godot.Collections.Array<Units.Type> heldUnits = new();
    private HashSet<Units.Type> heldUnitsInteral = new();
    
    public bool isReady = false;
    protected Units.Type chosenUnit = Units.Type.None;

    public override void _Ready()
    {
        heldUnitsInteral = heldUnits.ToHashSet();
        heldUnits.Clear();
        aiMemory = new Memory(this);
        
        currentHealth = maxHealth;
        healthBar.MaxValue = maxHealth;
        healthBar.Value = currentHealth;
    }

    public void SetAI(bool value) => isAI = value;

    public void SetDisplayName(string value)
    {
        name = value;
        nameLabel.Text = name;
    }

    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHealth() => currentHealth;
    public HashSet<Units.Type> GetHeldUnits() => heldUnitsInteral;
    public Units.Type GetChosenUnit() => chosenUnit;
    public void ResetChosenUnit() => chosenUnit = Units.Type.None;

    public void TakeDamageFrom(Units.Type from)
    {
        UnitData fromData = UnitRegistry.GetData(from);
        if (fromData == null)
            return;

        float baseDamage = fromData.damage;

        float strengthModifier = 1.0f;
        if (UnitRegistry.IsStrongAgainst(from, chosenUnit))
            strengthModifier = 1.25f;
        else if (UnitRegistry.IsWeakAgainst(from, chosenUnit))
            strengthModifier = 0.75f;

        float dayNightModifier = GetModifierValue(fromData, Modifiers.GetDayNight());
        float weatherModifier = GetModifierValue(fromData, Modifiers.GetWeather());

        int attackerIndex = playerIndex == 1 ? 2 : 1;
        float terrainModifier = GetModifierValue(fromData, Modifiers.GetTerrain(attackerIndex));

        float totalDamage = baseDamage * (strengthModifier + dayNightModifier + weatherModifier + terrainModifier);

        currentHealth -= totalDamage;
        healthBar.Value = currentHealth;

        string attackerName = playerIndex == 1 ? "Enemy" : "Player";
        Debug.Print(attackerName + " dealt " + totalDamage + " damage. ("
            + "Base: " + baseDamage
            + ", Strength: " + strengthModifier
            + ", DayNight: " + dayNightModifier
            + ", Weather: " + weatherModifier
            + ", Terrain: " + terrainModifier + ")");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Print(name + " lost.");
        }
    }

    private float GetModifierValue(UnitData data, Modifiers.Type mod)
    {
        if (mod == Modifiers.Type.NoWeather || mod == Modifiers.Type.NoTerrain || !data.buffnerfs.ContainsKey(mod))
            return 0.0f;
        return (Modifiers.Effects)data.buffnerfs[mod] == Modifiers.Effects.Buff ? 0.25f : -0.25f;
    }
    
    public virtual void ProcessTurn(double delta) {}
}