using Godot;
using System;

public partial class EnemyCombatant : Combatant
{
	[Export]
	EnemyNameplate nameplate;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		nameplate.UpdateHP(hp, maxHP);
		nameplate.SetComName(name);
		nameplate.SetNamePlateVisible(false);
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

	public override void SetTargetGUIElements(bool state){
		base.SetTargetGUIElements(state);
		nameplate.SetNamePlateVisible(state);
	}

	public override void TakeDamage(int damage){
		base.TakeDamage(damage);
		nameplate.UpdateHP(hp, maxHP);
	}
}

public class ActionLogicNotDefinedException : Exception{
	public ActionLogicNotDefinedException(string message) : base(message){}
}