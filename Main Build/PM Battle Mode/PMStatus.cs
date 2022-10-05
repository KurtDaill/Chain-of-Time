using Godot;
using System;
using static PMBattleUtilities;

public class PMStatus{
    //Number of turn this effect will last, set to -1 for effects that have to end at end of turn
    private int duration;
    private StatusEffect EventType;

    //Character that this effect is applied to. Left null for battlefield effects.
    private PMCharacter target = null;

    public PMStatus(StatusEffect effect, Targeting targetChar, int dur = -1){
        EventType = effect;
        duration = dur;
        //TODO implement TargetingRule to setting a parent
        
        target.AddStatus(this);
    }

    //Returns true when done.
    public virtual bool Execute(){
        return true;
    }

    public StatusEffect GetEventType(){
        return EventType;
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