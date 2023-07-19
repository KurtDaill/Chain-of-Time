using System;
using Godot;
using static BattleUtilities;

public partial class PlayerSwap : PlayerAbility
{
    
    private Roster battleRoster;
    private BattlePosition targetPos;

    public bool frameBuffer; //This is temp code; Remove when there are swap animations again
    public override void _Ready(){	
		name = "SWAP";
		rulesText = "";
		AbilityTargetingLogic = TargetingLogic.Special;
	}

    public override void _Process(double delta){  //This is temp code; Remove when there are swap animations again
        if(frameBuffer){
            frameBuffer = false;
            EmitSignal(CombatAction.SignalName.ActionComplete);
        }
    }

    public void SetupSwapDetails(Roster ros, BattlePosition pos){
        target = new Combatant[0];
        this.battleRoster = ros;
        targetPos = pos;
    }

    public async override void Begin(){
		base.Begin();
        frameBuffer = true;  //This is temp code; Remove when there are swap animations again
        battleRoster.SwapCharacters(source.GetPosition(), targetPos);
        //await ToSignal(battleRoster.GetAnimationPlayer(), AnimationPlayer.SignalName.AnimationFinished);
        EmitSignal(CombatAction.SignalName.ActionComplete);
	}   

    public void SetTargetPos(BattlePosition position){
        targetPos = position;
    }
    
	public override (Combatant, BattlePosition)[] GetPositionSwaps(){
		return new (Combatant, BattlePosition)[]{(source, targetPos)};
	}
}