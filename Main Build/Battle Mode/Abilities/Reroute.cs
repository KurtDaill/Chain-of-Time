using Godot;
using System;
using static BattleUtilities;
public partial class Reroute : PlayerSkill
{
	[Export(PropertyHint.File)]
	private string spellEffectFilePath;
	private PackedScene spellEffectPacked;
	public override void _Ready(){
		base._Ready();
		name = "Reroute";
		animation = "Spellcast";
		align = BattleUtilities.AbilityAlignment.Magic;
		skillType = "Spell"; 
		rulesText = "[center] Sends Target Ally to the First Rank";
		AbilityTargetingLogic = BattleUtilities.TargetingLogic.SinlgeTargetPlayer;
		spellEffectPacked = GD.Load<PackedScene>(spellEffectFilePath);
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
		switch(phase){
			case 0 : 	
				SpawnEffectOnTarget(2, spellEffectPacked, source); break;
			case 1 : 
				if(target[0].GetPosition().GetRank() == BattleRank.HeroBack){
					parentBattle.GetRoster().SwapCharacters(target[0], new BattlePosition(target[0].GetPosition().GetLane(), BattleRank.HeroMid));
					WaitForSwap();
					flagsRequiredToComplete[1] = false;
					parentBattle.GetRoster().SwapCharacters(target[0], new BattlePosition(target[0].GetPosition().GetLane(), BattleRank.HeroFront));
				}
				else
				{
					parentBattle.GetRoster().SwapCharacters(target[0], new BattlePosition(target[0].GetPosition().GetLane(), BattleRank.HeroFront));
				}
				WaitForSwap();
				break;
		}
    }

    public async void WaitForSwap(){
		//await ToSignal((Roster)GetParent().GetParent().GetParent(), Roster.SignalName.SwapComplete); Uncomment when swap animations are back!
		flagsRequiredToComplete[1] = true;
	}

	public override (Combatant, BattlePosition)[] GetPositionSwaps(){
		if(target[0].GetPosition().GetRank() == BattleRank.HeroBack){
			return new (Combatant, BattlePosition)[2]{(target[0], new BattlePosition(target[0].GetPosition().GetLane(), BattleRank.HeroMid)), (target[0], new BattlePosition(target[0].GetPosition().GetLane(), BattleRank.HeroFront))};
		}	
		return new (Combatant, BattlePosition)[1]{(target[0], new BattlePosition(target[0].GetPosition().GetLane(), BattleRank.HeroFront))};
	}
}
