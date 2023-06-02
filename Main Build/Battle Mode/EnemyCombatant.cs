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
	public virtual CombatEventData DecideAction(Battle parentBattle){
		//GD.Print("Not yet buddy boy");
		throw new ActionLogicNotDefinedException("Base EnemyCombatantDecideActionEnemies must all have an override of DecidedAction to define their attacking logic.");
		//return null;
	}
}

public class ActionLogicNotDefinedException : Exception{
	public ActionLogicNotDefinedException(string message) : base(message){}
}