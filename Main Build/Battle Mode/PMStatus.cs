using Godot;
using System;
using static PMBattleUtilities;

public partial class PMStatus : Node {
    //Number of turn this effect will last, set to -1 for effects that have to end at end of turn
    [Export]
    private int duration;
    [Export]
    private int magnitude;
    [Export]
    private StatusEffect statusType;
    [Export(PropertyHint.File)]
    private string particleResourcePath;

    //These two filepaths link to the larger icon used to indicate this status effect on the player readout
    //and the other smaller icon used when the readout has many status effects, and for display on enemy characters
    [Export(PropertyHint.File)]
    private string longIconPath;
    [Export(PropertyHint.File)]
    private string shortIconPath;
    [Export(PropertyHint.File)]
    private string enemyTexturePath;
    [Export]
    private StatusNotification notification;
    [Export]
    private StatusEffectParticles particles;

    //Character that this effect is applied to. Left null for battlefield effects.
    private PMCharacter target = null;
    private AnimationPlayer animPlayer;
    public void SetCustom(int dur, int mag){
        if(dur != -1) duration = dur;
        if(mag != -1) magnitude = mag;
    }

    //Returns true when done.
    public virtual bool Execute(){
        if(duration > 0){
            duration --;
        }
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

    public StatusNotification GetNotification(){
        return notification;
    }

    public void StartUpkeep(){
        particles.GlobalPosition = target.GetBodyRegion(particles.GetSpawnRegion()).GlobalPosition;
        target.GetBodyRegion(particles.GetSpawnRegion()).AddChild(particles);
        animPlayer.Play("Upkeep");
    }

    public void InflictDamage(){//TODO Add Functionality here for freeze and burn to deal damage
        
    }
    public void Expire(){
        //The Expire animation has to end with calling Finish() in order to properly delete this.
        animPlayer.Play("Expire");
    }

    public void Finish(){
        target.RemoveStatus(this);
        foreach(Node n in target.GetBodyRegion(particles.GetSpawnRegion()).GetChildren()){
            if(n == particles) n.QueueFree();
        }
        this.QueueFree();
    }
    public void Setup(PMCharacter tar){
        target = tar;
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        //animPlayer.Play("Apply");
    }

    public TextureRect GetLongIcon(){
        return ResourceLoader.Load<PackedScene>(longIconPath).Instantiate<TextureRect>();
    }

    public TextureRect GetShortIcon(){
        return ResourceLoader.Load<PackedScene>(shortIconPath).Instantiate<TextureRect>();
    }

    public Texture2D GetEnemyTexture(){
        return ResourceLoader.Load<Texture2D>(enemyTexturePath);
    }

    public void PlayNotification(Node3D spawnPoint){
        notification.PlayNotification(spawnPoint);
    }
}