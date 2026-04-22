using Godot;
using System;
using System.Collections.Generic;

public partial class Unit : Sprite2D
{
	[Export]
	public UnitData data { get; set; }
	public Player player { get; set; }
	public void SetPlayer(Player _player)
	{
		player = _player;
	}
	public bool IsStrongAgainst(Unit other)
	{
		if (data == null || other.data == null) return false;
		return data.strengths.Contains(other.data.unitName);
	}
	public bool IsWeakAgainst(Unit other)
	{
		if (data == null || other.data == null) return false;
		return data.weaknesses.Contains(other.data.unitName);
	}
	public void Damage (Unit from)
	{
		if (IsStrongAgainst(from))
		{
			from.data.damage *= 0.75f; // Take less damage from weeknesses
		}
		else if (IsWeakAgainst(from))
		{
			from.data.damage *= 1.25f; // Take more damage from strengths
		}
		TakeDamage(from.data.damage);
	}
	public float TakeDamage(float damage)
	{
		if (player == null) return 0;
		player.health -= damage;
		if (player.health < 0) player.health = 0;
		return player.health;
	}
}