using System;
using Godot;
using static BattleUtilities;

public partial class PositionSwap : PlayerAbility
{
    /*
    private Roster battleRoster;
    public PositionSwap(){	
		name = "SWAP";
		rulesText = "";
		AbilityTargetingLogic = TargetingLogic.AnyAlly;
	}

    public void SetupSwapDetails(Roster ros, Combatant tar){
        target = new Combatant[1];
        this.battleRoster = ros;
        this.target[0] = tar;
    }

    public async override void Begin(){
		base.Begin();
        battleRoster.SwapCharacters(source.GetPosition(), target[0].GetPosition());
        await ToSignal(battleRoster.GetAnimationPlayer(), AnimationPlayer.SignalName.AnimationFinished);
        EmitSignal(CombatAction.SignalName.ActionComplete);
	}   
    
	public override (Combatant, BattleUtilities.BattleRank)[] GetPositionSwaps(){
		return new (Combatant, BattleUtilities.BattleRank)[]{(source, target[0].GetPosition())};
	}
    */
}