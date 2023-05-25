using Godot;
using System;

public partial class EnemyCombatant : Combatant
{
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

	//Decides what action to take this round, returning the animation that matches said action
	public CombatEventData DecideAction(Battle parentBattle){
		GD.Print("Not yet buddy boy");
		return null;
	}
}
