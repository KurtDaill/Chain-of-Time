using Godot;
using System;
using static PMBattleUtilities;

public class PMStatus : Node{
    //Number of turn this effect will last, set to -1 for effects that have to end at end of turn
    [Export]
    private int duration;
    [Export]
    private int magnitude;
    [Export]
    private StatusEffect statusType;

    //Character that this effect is applied to. Left null for battlefield effects.
    private PMCharacter target = null;
    private AnimationPlayer animPlayer;

    public override void _Ready()
    {
        base._Ready();
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }
    public void SetCustom(int dur, int mag){
        if(dur != -1) duration = dur;
        if(mag != -1) magnitude = mag;
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

    public int GetMagnitude(){
        return magnitude;
    }
    public void Apply(){
        animPlayer.Play("Apply");
    }

    public void StartUpkeep(){
        animPlayer.Play("Upkeep");
    }

    public void InflictDamage(){//TODO Add Functionality here for freeze and burn to deal damage
        
    }
    public void Expire(){
        //The Expire animation has to end with calling RemoveYourself() in order to properly delete this.
        animPlayer.Play("Expire");
    }

    public void RemoveYourself(){
        this.QueueFree();
    }
}