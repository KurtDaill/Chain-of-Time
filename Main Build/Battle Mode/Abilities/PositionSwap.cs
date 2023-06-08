using System;
using Godot;
using static BattleUtilities;

public partial class PositionSwap : PlayerAbility
{
    private Roster battleRoster;
    private BattlePosition targetPos;
    public PositionSwap(){	
		name = "SWAP";
		animation = "SWAP";
		rulesText = "";
		AbilityTargetingLogic = TargetingLogic.Self;
	}

    public void SetupSwapDetails(Roster ros, BattlePosition tar){
        this.battleRoster = ros;
        this.targetPos = tar;
    }   

    public override async void Activate(int phase){
        if(phase != 0) throw new ArgumentException("The Swap ability must be activated at phase 0");
        battleRoster.SwapCharacters(source.GetPosition(), targetPos);
        await ToSignal(battleRoster.GetAnimationPlayer(), AnimationPlayer.SignalName.AnimationFinished);
        //When we're finished, we emit this signal because it's what Battle is listening for.
        source.GetAnimationPlayer().EmitSignal(AnimationPlayer.SignalName.AnimationFinished);
	}   
}