using Godot;
using System;
using static BattleUtilities;
public partial class LesserConfusion : EnemyAbility
{
	public override void _Ready(){
        name = "LesserConfusion";
        animation = "Spellcast";
		flagsRequiredToComplete = new bool[]{false, false};
		AbilityTargetingLogic = BattleUtilities.TargetingLogic.PlayerFront;
    }

	public override void Begin(){
		base.Begin();
		PlayCoreAnimation();
	}
    public override void AnimationTrigger(int phase)
	{
		base.AnimationTrigger(phase);
		parentBattle.GetRoster().SwapCharacters(BattlePosition.HeroFront, BattlePosition.HeroMid);
		WaitForSwap();
	}

	public async void WaitForSwap(){
		await ToSignal((Roster)GetParent().GetParent().GetParent(), Roster.SignalName.SwapComplete);
		flagsRequiredToComplete[1] = true;
	}
}
