using System;
using Godot;
using static BattleUtilities;
public partial class CatoTaunt : PlayerSkill{
    [Export(PropertyHint.File)]
    string tauntStatusEffectPath;
	public override void _Ready(){
		base._Ready();
		name = "Taunt";
		animation = "Taunt";
		align = BattleUtilities.AbilityAlignment.Normal;
		skillType = "Skill"; 
		rulesText = "[textSize]small[center] Cato TAUNTS until he moves, bringing target enemy to the front rank";
		AbilityTargetingLogic = BattleUtilities.TargetingLogic.SingleTargetEnemy;

		//The 0 entry of this array is always reserved for the core animation of this combat action
		//The 1 slot is for waiting for the opponent to be swapped
		flagsRequiredToComplete = new bool[]{false, false};
	}

	public override void Begin(){
		base.Begin();
		PlayCoreAnimation();
	}

    public override void Setup(Combatant proposedSource){
        base.Setup(proposedSource);
    }

	public override void AnimationTrigger(int phase)
	{
		base.AnimationTrigger(phase);
		StatusTauntUntilMove tauntStatus = GD.Load<PackedScene>(tauntStatusEffectPath).Instantiate<StatusTauntUntilMove>();
		source.GainStatus(tauntStatus);
		tauntStatus.Begin();
		if(target[0].GetPosition().GetRank() != BattleRank.EnemyFront){
			parentBattle.GetRoster().SwapCharacters(target[0].GetPosition(), new BattlePosition(target[0].GetPosition().GetLane(), BattleRank.EnemyFront));
        	WaitForSwap();
		}else{
			flagsRequiredToComplete[1] = true;
		}
	}   

	public async void WaitForSwap(){
		await ToSignal((Roster)GetParent().GetParent().GetParent(), Roster.SignalName.SwapComplete);
		flagsRequiredToComplete[1] = true;
	}

	public override (Combatant, BattlePosition)[] GetPositionSwaps(){
		if(target[0].GetPosition().GetRank() != BattleUtilities.BattleRank.EnemyFront) return new (Combatant, BattlePosition)[]{(target[0], new BattlePosition(target[0].GetPosition().GetLane(), BattleRank.EnemyFront))};
		return null;
	}
}