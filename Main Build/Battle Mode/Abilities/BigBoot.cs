using Godot;
using System;
using static BattleUtilities;
public partial class BigBoot : PlayerSkill
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		name = "Pulse Blast";
		animation = "Big Boot";
		align = BattleUtilities.AbilityAlignment.Normal;
		skillType = "Attack"; 
		rulesText = "[center] Deals 5 Damage and sends Silver back one rank";
		AbilityTargetingLogic = BattleUtilities.TargetingLogic.SingleTargetEnemy;
		flagsRequiredToComplete = new bool[]{false, false};
	}
	
	public override void Begin(){
		base.Begin();
		PlayCoreAnimation();
	}

	public override void AnimationTrigger(int phase){
		switch(phase){
			case 0 : target[0].TakeDamage(5); break;
			case 1 : 
				parentBattle.GetRoster().SwapCharacters(source.GetPosition(), new BattlePosition(source.GetPosition().GetLane(), source.GetPosition().GetRank()));
				WaitForSwap();
				//We shouldn't have triggered the flag for completing the core animation (it should be made long enough to not finish before the swap), so Big Boot Recovery finishing should be what trips that flag.
				source.GetAnimationPlayer().Play("Big Boot Recovery");
				break;
			default : throw new ArgumentException("Player Skill Big Boot only has phases 0 & 1, given phase value out of range.");
		}
	}

    public async void WaitForSwap(){
		await ToSignal((Roster)GetParent().GetParent().GetParent(), Roster.SignalName.SwapComplete);
		flagsRequiredToComplete[1] = true;
	}

	public override (Combatant, BattlePosition)[] GetPositionSwaps(){
		return new (Combatant, BattlePosition)[]{(source, source.GetPosition())};	
	}
}