using Godot;
using System;
using static PMBattleUtilities;
using System.Collections.Generic;
using System.Linq;

/*
This Class Represents an Attack, Spell, or other action a character can take on their turn, functioning with an attached animation 
player as a store of that action's data (i.e. damage, alignment, input required for player attacks, etc.)

This class acts as an attachment to a child node to that character, who keeps all of their abilities as children.
the functions provided in this class and it's abilities are intended to be called from animation players on Method Call Tracks

For Example: An animation  might have a "frames" track playing an actual animation of the character, and at a moment of impact
a method call track calls "Deal Damage"

The Exported Lists (aligns, targeting, etc.) allow developers to specify a number of values for various parts of the ability, called "events"
Later methods can have 'events numbers' passesd as arguments to look up the correct effect's information.
For Example, an attack may deal 3 instances of damage with different properties. So aligns might have {"Normal", "Normal", "Magic"} if only
the third attack is magical, and "effectValue" might have {8, 10, 20} if that each instance scales up.
*/

public partial class  PMBattleAbility : Node
{

    [Export]
    protected int coolDown = -1;

    [Export]
    protected int limitedAmmo = -1;

    [Export]
    public string name = "Ability";

    [Export]
    protected string firstAnimation = "???";

    public PMCharacter[] target;

    [Export]
    protected Godot.Collections.Array<NodePath> eventIndex;

    [Export]
    protected TargetingRule targetingRule = TargetingRule.Self;

    [Export]
    protected bool canTargetFliers = false;

    [Export]
    AbilityAlignment alignment = AbilityAlignment.Normal;

    [Export]
    protected bool usesEmpowered = true;
    [Export]
    protected bool consumesOvercharged = true;
    
    protected AbilityEvent[] events;
    public PMCharacter source;
    protected AnimationPlayer animPlay;

    //Is this ability done running for this iteration
    protected bool complete;
    
    protected int critDamage = -1;

    protected int failDamage = -1;

    protected bool successfulDefense = false;

    public override void _Ready()
    {
        animPlay = (AnimationPlayer) GetNode("AnimationPlayer");
        source = GetNode<PMCharacter>("..");
        events = new AbilityEvent[eventIndex.Count];
        for(int i = 0; i < eventIndex.Count; i++){
            events[i] = GetNode<AbilityEvent>(eventIndex[i]);
        }
    }
    public bool CheckForCompletion(){
        return complete;
    }

    public TargetingRule GetTargetingRule(){
        return targetingRule;
    }
    public void Begin(){
        complete = false;
        if(limitedAmmo != -1){
            if(limitedAmmo == 0){
                throw new OverflowException(); //TODO write a custom Exception
            }
            limitedAmmo--;
        }
        animPlay.Play(firstAnimation);
    }

    
    public void SetTargets(PMCharacter[] target){
        this.target = target;
        foreach(AbilityEvent ev in events){
            ev.SetTarget(this);
        }
    }
    

    public virtual void FinishSequence(){
        complete = true;
        critDamage = -1;
        failDamage = -1;
        source.ResetToIdleAnim();
    }

    protected virtual void DealDamage(int effectNum){
        string logOutput = name + " Dealt ";
        int dmg = events[effectNum].GetValue();
        AbilityAlignment damageType = events[effectNum].GetAlignment();
        if(critDamage != -1){
            dmg = critDamage;
        }
        if(failDamage != -1){
            dmg = failDamage;
        }else{ //Bonuses aren't applied to fail damage
            if(source.GetMyStatuses().Contains(StatusEffect.Empowered) && usesEmpowered){
                dmg += source.statusEffects.Where<PMStatus>(x => x.GetStatusType() == StatusEffect.Empowered).ToArray<PMStatus>()[0].GetMagnitude();
            }
            if(source.GetMyStatuses().Contains(StatusEffect.Overcharged) && consumesOvercharged){
                dmg += source.statusEffects.Where<PMStatus>(x => x.GetStatusType() == StatusEffect.Overcharged).ToArray<PMStatus>()[0].GetMagnitude();
            }
        }
        if(successfulDefense) dmg -= events[effectNum].GetDefenseFactor(); //Resolve weirdness for checking for player and enemy specific stuff in this class
        logOutput += dmg + " Damage to ";
        int targs = 0;
        foreach(PMCharacter character in events[effectNum].GetTargets()){
            if(character != null){
                character.TakeDamage(dmg, damageType);
                source.parentBattle.UpdateDamageScoreboard(dmg, source);
                targs ++;
            }
        }
        logOutput += targs + " Target(s).";
        GD.Print(logOutput);
    }

    public void Heal(int eventNum){ //TODO Write Me
        foreach(PMCharacter character in events[eventNum].GetTargets()){
            character.TakeHealing(events[eventNum].GetValue(), events[eventNum].GetAlignment());
            GD.Print(name + " Healed " + character.GetCharacterName() + " for " + events[eventNum].GetValue() + " HP.");
        }
    }

    public void InflictStatus(int eventNum){ //TODO Write Me
        /*
            foreach(Target)
                Load the PackedScene
                Instance the PackedScene
                Done inside of Event > Set Variables
        */
        var statusEvent = (AbilityEventStatusEffect)events[eventNum];
        foreach(PMCharacter target in events[eventNum].GetTargets()){
            var stat = statusEvent.InstanceStatusEffect(target);
            target.AddStatus(stat);
            target.parentBattle.LogStatusEffect(stat);
        }
    }   

    public void SpawnNode(string path){
        PackedScene scene = (PackedScene) GD.Load(path);
        Node effect = scene.Instantiate(PackedScene.GenEditState.Instance);
        AddChild(effect);
    }

    public bool CanTargetFliers(){
        return canTargetFliers;
    }

    public virtual void ExecuteEvent(int eventNum){
       switch(events[eventNum].GetEventType()){
            case EventType.Damage:
                DealDamage(eventNum);
                break;
            case EventType.Status:
                InflictStatus(eventNum);
                break;
            case EventType.Healing:
                Heal(eventNum);
                break;
        }
    }
    public string GetAbilityName(){
        return name;
    }

    public AbilityAlignment GetAbilityAlignment(){
        return alignment;
    }

    //This function changes a keyframe on a "transform" track to the transform of our currently selected target
    //A use case would be making a projectile thrown by this character properly hit it's target
    public void SetKeyTransToSelectedTarget(int trackIndex, int keyIndex, string animation){
        if(target.Length > 1 ) throw new NotImplementedException(); //TODO: Write Custom exception, can't use this function with multiple targets selected
        Vector3 targetTransform = this.target[0].GlobalPosition - source.GlobalPosition;
        animPlay.GetAnimation(animation).TrackSetKeyValue(trackIndex, keyIndex, targetTransform);//TODO: Convert between local coordinates
    }   

    public void SetKeyTransToSelectedTargetBezier(int xTrack, int yTrack, int zTrack, int keyIndex, string animation){
        if(target.Length > 1 ) throw new NotImplementedException(); //TODO: Write Custom exception, can't use this function with multiple targets selected
        Vector3 targetTransform = this.target[0].GlobalPosition - source.GlobalPosition;
        Animation anim= animPlay.GetAnimation(animation);
        anim.BezierTrackSetKeyValue(xTrack, keyIndex, targetTransform.x);//TODO: Convert between local coordinates
        anim.BezierTrackSetKeyValue(yTrack, keyIndex, targetTransform.y);//TODO: Convert between local coordinates
        anim.BezierTrackSetKeyValue(zTrack, keyIndex, targetTransform.z);//TODO: Convert between local coordinates
    }
}

