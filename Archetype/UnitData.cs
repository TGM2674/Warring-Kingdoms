using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class UnitData : Resource
{
	[Export]
	public Main.Units unitName { get; set; } = Main.Units.Archers;
	[Export]
	public float damage { get; set; } = 100;
	[Export]
	public Godot.Collections.Array<Main.Units> strengths { get; set; } = new Godot.Collections.Array<Main.Units>();
	[Export]
	public Godot.Collections.Array<Main.Units> weaknesses { get; set; } = new Godot.Collections.Array<Main.Units>();
	[Export]
	public Godot.Collections.Dictionary<Main.Modifiers, Main.ModifierEffects> buffnerfs { get; set; } = new Godot.Collections.Dictionary<Main.Modifiers, Main.ModifierEffects>();
}