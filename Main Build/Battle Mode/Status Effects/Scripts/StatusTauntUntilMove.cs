using System;
using Godot;
using static BattleUtilities;
public partial class StatusTauntUntilMove : StatusTaunting
{
    private BattlePosition ogPos;
    public override void _Ready(){
        base._Ready();
        name = "Taunt";
        animation = null;
        startingDuration = -1;
    }

    public override void _Process(double delta){
        if(running){
            EmitSignal(CombatAction.SignalName.ActionComplete);
            running = false;
        }
        if(source.GetPosition() != ogPos){
            source.LogExpiredStatus(this);
            this.QueueFree();
        }
    }

    public override void Setup(Combatant proposedSource){
        base.Setup(proposedSource);
        ogPos = source.GetPosition();
    }
}