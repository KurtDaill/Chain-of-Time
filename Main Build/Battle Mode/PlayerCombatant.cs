using Godot;
using System;
using System.Collections.Generic;
public partial class PlayerCombatant : Combatant
{
	protected int SP;

	//Includes a dicitonary of potential damage done by a basic attack (expressed in an int), and the probability of that ammount of damage (expressed in a double)
	//The doubles should add up to one.
	protected Dictionary<double, int> basicAttackDamageRange;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
