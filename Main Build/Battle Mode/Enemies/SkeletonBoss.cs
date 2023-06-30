using Godot;
using System;

public partial class SkeletonBoss : EnemyCombatant
{
	[Export]
	private EnemyAbility attack;
	[Export]
	private EnemyAbility rally;
	bool hasRallied = false;
	public override void _Ready()
	{
        name = "Skeleton";
        base._Ready();
		attack.Setup(this);
	}


	public override CombatEventData DecideAction(Battle parentBattle){
		if(parentBattle.GetRoster().GetAllEnemyCombatants().Length > 1 && !hasRallied){

		}
        attack.SetTargets(parentBattle.GetRoster().GetAllPlayerCombatants());
        return attack.GetEventData();
    }  
}
