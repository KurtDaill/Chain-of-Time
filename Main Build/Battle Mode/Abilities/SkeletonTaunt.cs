using Godot;
using System;

public partial class SkeletonTaunt : EnemyAbility
{
	[Export(PropertyHint.File)]
    string tauntStatusEffectPath;
	public override void _Ready(){
		base._Ready();
		name = "Taunt";
		animation = "Taunt";
		AbilityTargetingLogic = BattleUtilities.TargetingLogic.Self;

		//The 0 entry of this array is always reserved for the core animation of this combat action
		//The 1 slot is for waiting on the 
		flagsRequiredToComplete = new bool[]{false};
	}

	public override void Begin(){
		base.Begin();
		PlayCoreAnimation();
	}
	
	public override void AnimationTrigger(int phase)
	{
		base.AnimationTrigger(phase);
		StatusTauntUntilMove tauntStatus = GD.Load<PackedScene>(tauntStatusEffectPath).Instantiate<StatusTauntUntilMove>();
		source.GainStatus(tauntStatus);
		tauntStatus.Begin();
	}  
}
