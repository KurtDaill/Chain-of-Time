using System;
using Godot;
using static BattleUtilities;

public partial class SkeletonHunter : EnemyCombatant 
{
	[Export]
	private EnemyAbility rangedAttack;
    [Export]
    private EnemyAbility meleeAttack;
	public override void _Ready()
	{
        name = "Skeleton";
        base._Ready();
		rangedAttack.Setup(this);
        meleeAttack.Setup(this);
	}


	public override CombatEventData DecideAction(Battle parentBattle){
        if(GetPosition().GetRank() == BattleRank.EnemyFront){
            meleeAttack.SetTargets(parentBattle.GetRoster().GetAllPlayerCombatants());
            return meleeAttack.GetEventData();
        }else{
            rangedAttack.SetTargets(parentBattle.GetRoster().GetAllPlayerCombatants());
            return rangedAttack.GetEventData();
        }
    }   

	protected override void OnAnimationComplete(StringName animName){
			animPlay.Play("Idle");
	}
}