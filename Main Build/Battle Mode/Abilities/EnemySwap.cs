using System;
using Godot;
using static BattleUtilities;

public partial class EnemySwap : EnemyAbility{
    private Roster battleRoster;
    public override void _Ready(){	
		AbilityTargetingLogic = TargetingLogic.Special;
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
    
	public override (Combatant, BattlePosition)[] GetPositionSwaps(){
		return new (Combatant, BattlePosition)[]{(source, target[0].GetPosition())};
	}
}