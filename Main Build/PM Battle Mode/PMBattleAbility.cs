using Godot;
using System;
using static PMBattleUtilities;
using System.Collections.Generic;
public class  PMBattleAbility : Node
{
    [Export(PropertyHint.Enum)]
    protected List<AbilityAlignment> aligns = new List<AbilityAlignment>(){AbilityAlignment.Normal};

    [Export(PropertyHint.Enum)]
    protected List<TargetingRule> targeting = new List<TargetingRule>{TargetingRule.Self};
    protected Targeting[] targets;

    [Export(PropertyHint.Enum)]
    protected List<EffectType> effects = new List<EffectType>{EffectType.Damage};

    [Export(PropertyHint.Enum)]
    protected List<StatusEffect> statusEffect = new List<StatusEffect>{StatusEffect.None};

    [Export]
    protected int[] effectValue = new int[]{0};

    [Export]
    protected List<Control> guiElements = new List<Control>();


    [Export]
    public string name = "Ability";

    [Export]
    private string firstAnimation = "???";
    
    protected PMCharacter source;
    protected AnimationPlayer animPlay;
    protected bool complete;

    public override void _Ready()
    {
        animPlay = (AnimationPlayer) GetNode("AnimationPlayer");
    }
    public bool CheckForCompletion(){
        return complete;
    }

    public void Begin(){
        complete = false;
        animPlay.Play(firstAnimation);
    }

    public void SetTargets(Targeting[] newTargets){
        targets = newTargets;
    }

    public void FinishSequence(string anim_name){
        complete = true;
    }

    public virtual void DealDamage(int effectNum){
    }

    public void Heal(int heal, int target = -1, int al = -1){
        //TargetingRule localTarget = ConvertTarget(target);
        //AbilityAlignment localAlign = ConvertAlignment(al);
    }

    public void InflictStatus(int status, int duration = 1, int target = -1, int al = -1){
        //TargetingRule localTarget = ConvertTarget(target);
        //AbilityAlignment localAlign = ConvertAlignment(al);
    }   

    private PMCharacter[] GetCharacter(TargetingRule target){
        PMBattle battle = (PMBattle) GetNode(PMBattleUtilities.pathToBattle);
        switch(target){
            case TargetingRule.Self : 
                return new PMCharacter[]{source};
            case TargetingRule.EnemyOne :
                return new PMCharacter[]{battle.GetEnemyCharacter(0)};
            case TargetingRule.EnemyTwo :
                return new PMCharacter[]{battle.GetEnemyCharacter(1)};
            case TargetingRule.EnemyThree :
                return new PMCharacter[]{battle.GetEnemyCharacter(2)};
            case TargetingRule.AllEnemy :
                return null;//TODO this function seems dumb as hell
        }
        return null;
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

