using Godot;
using System;
using System.Collections.Generic;
using static PMBattleUtilities;


//TODO Add Logic to prevent a character from Taunting and Being Invisible or Phased Out
//Possible have a taunt status stay up, but not effect anything while Invisible or PhasedOut
public class PMCharacter : Node{

    public List<PMStatus> statusEffects;
    public PMBattle parentBattle;

    [Export]
    protected int MaxHP;

    protected int currentHP, damageTakenThisTurn;

    [Export(PropertyHint.Enum)]
    public List<AbilityAlignment>ModifiedDamageTypes;

    public BattlePos myPosition;

    [Export]
    public List<float> Modifier;
    [Export]
    string name;
    public Dictionary<AbilityAlignment, float> DamageModifiers = new Dictionary<AbilityAlignment, float>();
    public AnimationPlayer animPlay;
    public override void _Ready(){
        statusEffects = new List<PMStatus>();
        parentBattle = (PMBattle) GetNode("/root/Battle");
        animPlay = GetNode<AnimationPlayer>("AnimationPlayer");
        currentHP = MaxHP;
    } 

    public override void _Process(float delta){
        
    }

    public bool IsBloodied(){
        return (currentHP <= (MaxHP/2));
    }

    public bool IsHurt(){
        return (currentHP < MaxHP);
    }

    public int GetHP(){
        return currentHP;
    }

    //Related whether this character is available to be targeted, taking whether the targeting can target fliers as an argument
    public bool IsTargetable(bool targetsFliers){
        bool targetable = true;
        foreach(PMStatus status in statusEffects){
            StatusEffect type = status.GetStatusType();
            if(type == StatusEffect.Invisible || type == StatusEffect.PhasedOut){
                targetable = false;
                break;
            }
            //if(!targetsFliers && statusEffects.Contains())
        }
        return targetable;
    }

    public void AddStatus(PMStatus newEffect){
        //Checks if the newEffect is another instance of a current effect, if so we keep the instance with more duration
        foreach(PMStatus oldEffect in statusEffects){
            if(newEffect.GetStatusType() == oldEffect.GetStatusType()){
                if(newEffect.GetDuration() > oldEffect.GetDuration()){
                    statusEffects.Remove(oldEffect);
                    statusEffects.Add(newEffect);
                    return;
                }else{
                    return;
                }
            }
        }

        statusEffects.Add(newEffect);
    }

    public virtual void TakeDamage(int damage, AbilityAlignment alignment){
        foreach(KeyValuePair<AbilityAlignment, float> mod in DamageModifiers){
            damage = Mathf.RoundToInt(damage * mod.Value);
        }
        this.currentHP -= damage;
        //TODO Add Damage Number System
        animPlay.Play("HitReact");
        GD.Print(name + " is hit for: " + damage); //TODO modify this to print to a combat log?
    }

    //Called by OnAnimationFinished Signal
    public void ReturnToIdle(string anim_name){
        animPlay.Play("Idle");
    }

    public void NewTurnUpkeep(){
        damageTakenThisTurn = 0;
    }

    public StatusEffect[] GetMyStatuses(){
        List<StatusEffect> statuses = new List<StatusEffect>();
        foreach(PMStatus statusObject in statusEffects){
            statuses.Add(statusObject.GetStatusType());
        }
        return statuses.ToArray();
    }

    public void ResetToIdleAnim(){
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
    }
}
