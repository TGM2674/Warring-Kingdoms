using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node
{
	public enum Units
	{
		Archers,
		Spearsmen,
		Swordsmen,
		Cavalries,
		Mages
	}
	public enum ModifierEffects
	{
		Buff,
		Nerf
	}
	public enum Modifiers
	{
		Day,
		Night,
		Raining,
		Snowing,
		HighGround,
		Swamp
	}
	[Export]
	private PackedScene archerScene = null;
	Player player = null;
	List<Unit> playerUnits = new List<Unit>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = new Player();
		AddChild(player);
		Archer archer = archerScene.Instantiate<Archer>();
		archer.SetPlayer(player);
		player.AddChild(archer);
		playerUnits.Add(archer);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}