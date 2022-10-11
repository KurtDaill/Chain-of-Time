using Godot;
using System;
using static PMBattleUtilities;

public class PMStatus{
    //Number of turn this effect will last, set to -1 for effects that have to end at end of turn
    private int duration;
    private StatusEffect statusType;

    //Character that this effect is applied to. Left null for battlefield effects.
    private PMCharacter target = null;

    public PMStatus(StatusEffect effect, BattlePos targetChar, int dur = -1){
        statusType = effect;
        duration = dur;
        //TODO implement TargetingRule to setting a parent
        
        target.AddStatus(this);
    }

    //Returns true when done.
    public virtual bool Execute(){
        return true;
    }

    public StatusEffect GetStatusType(){
        return statusType;
    }

    public int GetDuration(){
        return duration;
    }
    public void Apply(){

    }

    public void Upkeep(){

    }

    public void Expire(){

    }
}