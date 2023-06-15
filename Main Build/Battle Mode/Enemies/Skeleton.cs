using Godot;
using System;

public partial class Skeleton : EnemyCombatant
{
	[Export]
	private EnemyAbility attack;
	public override void _Ready()
	{
        name = "Skeleton";
        base._Ready();
		attack.Setup(this);
	}

	public override void _Process(double delta)
	{
	}

	public override CombatEventData DecideAction(Battle parentBattle){
        attack.SetTargets(parentBattle.GetRoster().GetAllPlayerCombatants());
        return attack.GetEventData();
    }   
}
