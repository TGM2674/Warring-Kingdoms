using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class UnitController : Node
{
    public string name = "";
    
    [Export] private Label nameLabel = null;
    [Export] public ProgressBar healthBar = null;
    
    private float maxHealth = 10000;
    private float currentHealth = 1;

    [Export] private bool isAI = false;
    public Memory aiMemory = null;
    
    [Export] private Godot.Collections.Array<Units.Type> heldUnits =  new();
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

    public void SetAI(bool value)
    {
        isAI = value;
    }

    public void SetName(string value)
    {
        name = value;
        nameLabel.Text = name;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
    
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public HashSet<Units.Type> GetHeldUnits()
    {
        return heldUnitsInteral;
    }

    public Units.Type GetChosenUnit()
    {
        return chosenUnit;
    }

    public void ResetChosenUnit()
    {
        chosenUnit = Units.Type.None;
    }

    public void TakeDamageFrom(Units.Type from)
    {
        UnitData fromData = UnitRegistry.GetData(from);
        if (fromData == null)
            return;
        
        float damage = fromData.damage;
        
        if (UnitRegistry.IsStrongAgainst(chosenUnit, from))
            damage *= 0.75f; // Take less damage if strong against this unit
        else if (UnitRegistry.IsWeakAgainst(chosenUnit, from))
            damage *= 1.25f; // Take more damage if weak against this unit
        
        currentHealth -= damage;
        healthBar.Value = currentHealth;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Print(name + " lost.");
        }
    }
    
    public virtual void ProcessTurn(double delta){}
}
