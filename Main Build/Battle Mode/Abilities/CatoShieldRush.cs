using System;
using Godot;
using static BattleUtilities;
public partial class CatoShieldRush : PlayerSkill{
	/*
    [Export(PropertyHint.File)]
    string armorStatusFilePath;

	public override void _Ready(){
		base._Ready();
		name = "Shield Rush";
		animation = "Shield Rush";
		align = BattleUtilities.AbilityAlignment.Normal;
		skillType = "Skill"; 
		rulesText = "[center] Cato rushes to the front rank, gaining 1 Armor for 3 Turns";
		AbilityTargetingLogic = BattleUtilities.TargetingLogic.Self;

		//The 0 entry of this array is always reserved for the core animation of this combat action
		//The 1 slot is for waiting for Cato to be swapped
		flagsRequiredToComplete = new bool[]{false, false};
	}

    public override void Begin()
    {
        base.Begin();
        PlayCoreAnimation();
    }

    public override void AnimationTrigger(int phase){
		base.AnimationTrigger(phase);
        
		if(target[0].GetPosition() != BattleRank.HeroFront){
			parentBattle.GetRoster().SwapCharacters(source.GetPosition(), BattleRank.HeroFront);
        	WaitForSwap();
		}else{
			flagsRequiredToComplete[1] = true;
		}
        StatusArmor armor = GD.Load<PackedScene>(armorStatusFilePath).Instantiate<StatusArmor>();
        source.GainStatus(armor);
    }

    public async void WaitForSwap(){
		await ToSignal((Roster)GetParent().GetParent().GetParent(), Roster.SignalName.SwapComplete);
		flagsRequiredToComplete[1] = true;
	}

	public override (Combatant, BattleUtilities.BattleRank)[] GetPositionSwaps(){
		if(source.GetPosition() != BattleRank.HeroFront) return new (Combatant, BattleUtilities.BattleRank)[]{(source, BattleUtilities.BattleRank.HeroFront)};
		return null;
	}
	*/
}