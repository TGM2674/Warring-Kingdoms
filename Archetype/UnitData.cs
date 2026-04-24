using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class UnitData : Resource
{
    [Export]
    public PackedScene UnitScene = null;
	[Export]
	public Units.Type unitName = Units.Type.Archers;
	[Export]
	public float damage = 100;
	[Export]
	public Godot.Collections.Array<Units.Type> strengths = new();
	[Export]
	public Godot.Collections.Array<Units.Type> weaknesses = new();
	[Export]
	public Godot.Collections.Dictionary<Modifiers.Type, Modifiers.Effects> buffnerfs = new();
}