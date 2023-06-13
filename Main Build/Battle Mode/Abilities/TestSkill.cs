using System;
using Godot;
using static BattleUtilities;
public partial class TestSkill : PlayerSkill{
	public override void _Ready(){
		base._Ready();
		name = "Swap 'Em";
		animation = "SpellCast";
		align = BattleUtilities.AbilityAlignment.Magic;
		skillType = "Spell"; 
		rulesText = "Swaps Hero 1 & Hero 2";
		AbilityTargetingLogic = BattleUtilities.TargetingLogic.Self;

		//The 0 entry of this array is always reserved for the core animation of this combat action
		//The 1 slot is for waiting on the 
		flagsRequiredToComplete = new bool[]{false, false};
	}

	public override void Activate(int phase)
	{
		base.Activate(phase);
		parentBattle.GetRoster().SwapCharacters(BattlePosition.HeroFront, BattlePosition.HeroMid);
		WaitForSwap();
	}

	public override void Run(){
		base.Run();
	}

	public async void WaitForSwap(){
		await ToSignal((Roster)GetParent().GetParent().GetParent(), Roster.SignalName.SwapComplete);
		flagsRequiredToComplete[1] = true;
	}
}
