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
    [Export(PropertyHint.Enum)]
    protected List<AbilityAlignment> aligns = new List<AbilityAlignment>(){AbilityAlignment.Normal};

    //Targeting refers to the data represeting what this attack *could* target
    [Export(PropertyHint.Enum)]
    protected TargetingRule targeting = TargetingRule.Self;

    //Targets refers to the exact targets that are being hit per effects
    protected Targeting[] targets = new Targeting[]{Targeting.EnemyOne}; 

    [Export(PropertyHint.Enum)]
    protected List<EventType> events = new List<EventType>{EventType.Damage};

    [Export(PropertyHint.Enum)]
    protected List<StatusEffect> statusEffect = new List<StatusEffect>{StatusEffect.None};

    [Export]
    protected int[] eventValue = new int[]{0};

    [Export]
    protected int coolDown = -1;

    [Export]
    protected int limitedAmmo = -1;

    [Export]
    public string name = "Ability";

    [Export]
    private string firstAnimation = "???";
    
    protected PMCharacter source;
    protected AnimationPlayer animPlay;

    //Is this ability done running for this iteration
    protected bool complete;
    
    protected int critDamage = -1;

    protected int failDamage = -1;

    public override void _Ready()
    {
        animPlay = (AnimationPlayer) GetNode("AnimationPlayer");
        source = GetNode<PMCharacter>("..");
    }
    public bool CheckForCompletion(){
        return complete;
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

    public void SetTargets(Targeting[] newTargets){
        targets = newTargets;
    }

    public TargetingRule GetTargetingRule(){
        return targeting;
    }

    public void FinishSequence(string anim_name){
        complete = true;
        critDamage = -1;
        failDamage = -1;
    }

    protected virtual void DealDamage(int effectNum){
        Targeting damageTargets = targets[effectNum];
        int dmg = eventValue[effectNum];
        AbilityAlignment damageType = aligns[effectNum];

        if(critDamage != -1){
            dmg = critDamage;
        }else if(failDamage != -1){
            dmg = failDamage;
        }
        
        foreach(Targeting target in Enum.GetValues(typeof(Targeting))){
            if((target & damageTargets) == target){
                PMCharacter character;
                character = source.parentBattle.TargetLookup(target);
                if(character != null){
                    character.TakeDamage(dmg, damageType);
                    source.parentBattle.UpdateDamageScoreboard(dmg, source);
                }
            }
        }
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

    

    public virtual void ExecuteEvent(int eventNum){
        switch(events[eventNum]){
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
    /*
    private TargetingRule ConvertTarget(int target){
        if(target == -1){
            //return targeting;
        }
        return (TargetingRule) target;
    }

    private AbilityAlignment ConvertAlignment(int alignment){
        if(alignment == -1){
            //return align;
        }
        return (AbilityAlignment) alignment;
    }
    */
}

