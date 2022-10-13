using Godot;
using System;
using static PMBattleUtilities;
using System.Collections.Generic;

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

public class  PMBattleAbility : Node
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
    protected List<NodePath> eventIndex;

    [Export]
    protected TargetingRule targetingRule = TargetingRule.Self;

    [Export]
    protected bool canTargetFliers = false;
    
    protected AbilityEvent[] events;
    public PMCharacter source;
    protected AnimationPlayer animPlay;

    //Is this ability done running for this iteration
    protected bool complete;
    
    protected int critDamage = -1;

    protected int failDamage = -1;

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
        //BattlePos[] damageTargets = events[effectNum].GetTargetPositions();
        int dmg = events[effectNum].GetValue();
        AbilityAlignment damageType = events[effectNum].GetAlignment();
       // AbilityAlignment damageType = aligns[effectNum];

        if(critDamage != -1){
            dmg = critDamage;
        }else if(failDamage != -1){
            dmg = failDamage;
        }
        foreach(PMCharacter character in events[effectNum].GetTargets()){
            if(character != null){
                character.TakeDamage(dmg, damageType);
                source.parentBattle.UpdateDamageScoreboard(dmg, source);
            }
        }
        /*
        for(int i = 0; i < damageTargets.Length; i++){
            PMCharacter ch = source.parentBattle.PositionLookup(damageTargets[i]);
            if(ch != null){
                ch.TakeDamage(dmg, damageType);
                source.parentBattle.UpdateDamageScoreboard(dmg, source);
            }
        }
        */
    }

    public void Heal(int eventNum){ //TODO Write Me
        //TargetingRule localTarget = ConvertTarget(target);
        //AbilityAlignment localAlign = ConvertAlignment(al);
    }

    public void InflictStatus(int eventNum){ //TODO Write Me
        //TargetingRule localTarget = ConvertTarget(target);
        //AbilityAlignment localAlign = ConvertAlignment(al);
    }   

    public void SpawnNode(string path){
        PackedScene scene = (PackedScene) GD.Load(path);
        Node effect = scene.Instance();
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
}

