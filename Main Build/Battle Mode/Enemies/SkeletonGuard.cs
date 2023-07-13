using Godot;
using System;
using static BattleUtilities;
using System.Linq;

public partial class SkeletonGuard : EnemyCombatant
{
	[Export]
	private EnemyAbility attack;
	[Export]
	private EnemyAbility taunt;
	public override void _Ready()
	{
        name = "Skeleton Guard";
        base._Ready();
		attack.Setup(this);
	}


	public override CombatEventData DecideAction(Battle parentBattle){
		if(taunt.GetenabledRanks().Contains(this.GetPosition().GetRank()) && activeStatuses.Where(x => x is StatusTaunting).Count() == 0){ //If we're not taunting and in the front
			return taunt.GetEventData();
		}
        attack.SetTargets(parentBattle.GetRoster().GetAllPlayerCombatants());
        return attack.GetEventData();
    }   

	protected override void OnAnimationComplete(StringName animName){
		if(activeStatuses.Where(x => x is StatusTaunting).Count() > 0){//if we're taunting
			animPlay.Play("TauntIdle");
		}else{
			animPlay.Play("Idle");
		}	
	}
}
