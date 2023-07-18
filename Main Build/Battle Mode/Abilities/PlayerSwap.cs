using System;
using Godot;
using static BattleUtilities;

public partial class PlayerSwap : PlayerAbility
{
    
    private Roster battleRoster;
    private BattlePosition targetPos;
    public override void _Ready(){	
		name = "SWAP";
		rulesText = "";
		AbilityTargetingLogic = TargetingLogic.Special;
	}

    public void SetupSwapDetails(Roster ros, BattlePosition pos){
        target = new Combatant[0];
        this.battleRoster = ros;
        targetPos = pos;
    }

    public async override void Begin(){
		base.Begin();
        battleRoster.SwapCharacters(source.GetPosition(), targetPos);
        await ToSignal(battleRoster.GetAnimationPlayer(), AnimationPlayer.SignalName.AnimationFinished);
        EmitSignal(CombatAction.SignalName.ActionComplete);
	}   
    
	public override (Combatant, BattlePosition)[] GetPositionSwaps(){
		return new (Combatant, BattlePosition)[]{(source, target[0].GetPosition())};
	}
}