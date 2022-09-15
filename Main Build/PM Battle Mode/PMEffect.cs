using Godot;
using System;
using static PMBattleUtilities;

public class PMEffect{
    //Number of turn this effect will last, set to -1 for effects that have to end at end of turn
    private int duration;
    private StatusEffect effectType;

    //Character that this effect is applied to. Left null for battlefield effects.
    private PMCharacater target = null;

    public PMEffect(StatusEffect effect, TargetRule targeting, int dur = -1){
        effectType = effect;
        duration = dur;
        //TODO implement TargetRule to setting a parent
        
        target.AddStatus(this);
    }

    //Returns true when done.
    public virtual bool Execute(){
        return true;
    }

    public StatusEffect GetEffectType(){
        return effectType;
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