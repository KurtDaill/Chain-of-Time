using System;
using Godot;
using static BattleUtilities;

public partial class PositionSwap : PlayerAbility
{
    private Roster battleRoster;
    private Combatant swapTarget;
    public PositionSwap(){	
		name = "SWAP";
		rulesText = "";
		AbilityTargetingLogic = TargetingLogic.AnyAlly;
	}

    public void SetupSwapDetails(Roster ros, Combatant tar){
        this.battleRoster = ros;
        this.swapTarget = tar;
    }

    public async override void Begin(){
		base.Begin();
        battleRoster.SwapCharacters(source.GetPosition(), swapTarget.GetPosition());
        await ToSignal(battleRoster.GetAnimationPlayer(), AnimationPlayer.SignalName.AnimationFinished);
        EmitSignal(CombatAction.SignalName.ActionComplete);
	}   
}