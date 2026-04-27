using Godot;

[GlobalClass]
public partial class UnitData : Resource
{
    [Export] public PackedScene UnitScene = null;
    [Export] public PackedScene AttackScene = null;
    [Export] public Units.Type unitName = Units.Type.Archers;
    [Export] public float damage = 1000;
    [Export] public Godot.Collections.Array<Units.Type> strengths = new();
    [Export] public Godot.Collections.Array<Units.Type> weaknesses = new();
    [Export] public Godot.Collections.Array<Modifiers.Type> buffs = new();
}