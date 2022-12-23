using Godot;
using System;
using System.Collections.Generic;
using static PMBattleUtilities;


//TODO Add Logic to prevent a character from Taunting and Being Invisible or Phased Out
//Possible have a taunt status stay up, but not effect anything while Invisible or PhasedOut
public partial class PMCharacter : Sprite3D{

    public List<PMStatus> statusEffects;
    public PMBattle parentBattle;

    [Export]
    protected int maxHP;

    protected int currentHP = -1;
    protected int damageTakenThisTurn;

    [Export(PropertyHint.EnumSuggestion)]
    public Godot.Collections.Array<AbilityAlignment>ModifiedDamageTypes;

    public BattlePos myPosition;

    [Export]
    public Godot.Collections.Array<float> Modifier;
    [Export]
    string name;
    public Dictionary<AbilityAlignment, float> DamageModifiers = new Dictionary<AbilityAlignment, float>();
    public AnimationPlayer animPlay;
    private bool doneDying = false;
    public Sprite3D pointerGraphic;
    [Export(PropertyHint.File)]
    public string HealingNumberResource = "res://PM Battle Mode/Healing Number.tscn";
    [Export(PropertyHint.File)]
    public string DamageNumberResource = "res://PM Battle Mode/Damage Number.tscn"; 

    private PackedScene damageNum, healingNum;
    public override void _Ready(){
        statusEffects = new List<PMStatus>();
        parentBattle = (PMBattle) GetNode("/root/Battle");
        animPlay = GetNode<AnimationPlayer>("AnimationPlayer");
        pointerGraphic = GetNode<Sprite3D>("Pointer");
        if(currentHP == -1) currentHP = maxHP;
        damageNum = GD.Load<PackedScene>(DamageNumberResource);
        healingNum = GD.Load<PackedScene>(HealingNumberResource);
    } 

    public override void _Process(double delta){
        
    }

    public bool IsBloodied(){
        return (currentHP <= (maxHP/2));
    }

    public bool IsHurt(){
        return (currentHP < maxHP);
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
                    oldEffect.QueueFree();
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
        var dmg = (Label3D) damageNum.Instantiate();
        dmg.Text = "" + damage;
        this.AddChild(dmg);
        if(damage > 0)animPlay.Play("HitReact");
        GD.Print(name + " is hit for: " + damage); //TODO modify this to print to a combat log?
    }

    public virtual void TakeHealing(int heal, AbilityAlignment alignment){
        if(heal <= 0) throw new NotImplementedException(); //TODO Write custom exception
        if(currentHP == 0){
            animPlay.Play("Revive");
        }
        var healing = (Label3D) healingNum.Instantiate();
        healing.Text = "" + heal;
        this.AddChild(healing);
        currentHP += heal;
        if(currentHP > maxHP) currentHP = maxHP;
    }

    //Called by OnAnimationFinished Signal
    public void ReturnToIdle(string anim_name){
        animPlay.Play("Idle");
    }

    //Runs any functionality required when the Battle Starts a new turn
    public void NewTurnUpkeep(){
        damageTakenThisTurn = 0;
    }

    public List<StatusEffect> GetMyStatuses(){
        List<StatusEffect> statuses = new List<StatusEffect>();
        foreach(PMStatus statusObject in statusEffects){
            statuses.Add(statusObject.GetStatusType());
        }
        return statuses;
    }

    public string GetCharacterName(){
        return name;
    }
    public void ResetToIdleAnim(){
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
    }

    public void SetPointerVisibility(bool set){
        pointerGraphic.Visible = set;
    }

    
    //Called by other nodes that need to animate this character for a while, and don't need their default animations breaking through
    public void StopAnimation(){
        animPlay.Stop();
    }
    //Restarts animation from StopAnimation
    public void ResumeAnimation(){
        animPlay.Play();
    }

    //Returns true when the Defeat Animation is complete
    public virtual bool DefeatMe(){
        if(animPlay.CurrentAnimation != "Defeat"){
            animPlay.Play("Defeat");
        }
        if(doneDying){
            return true;
        }else{
            return false;
        }
    }

    public virtual void FinishDefeat(){
        doneDying = true;
    }
}
