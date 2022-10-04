using Godot;
using System;
using System.Collections.Generic;
using static PMBattleUtilities;

public class PMPlayerAbility : PMBattleAbility
{

    private bool waitingForInput;
    private string targetInput, targetAnimation;
    private bool inHoldAttack, attackReady, criticalTiming, activate;
    private int critDamage = -1;

    private int failDamage = 0;
    private int currentEffect = 0; 

    //Used for timing inputs
    //activate is when the input will be acepted, perfect is start of optimal time, late ends optimal time, and end is when the input is no longer accepted
    //private int activateTime, perfectTime, lateTime, endTime;

    public override void _Process(float delta)
    {
        if(waitingForInput){
            if(Input.IsActionJustPressed(targetInput)){
                waitingForInput = false;
                animPlay.Play();
            }
            return;
        }

        if(inHoldAttack){
            if(!Input.IsActionPressed(targetInput)){
                animPlay.Play(targetAnimation);
                inHoldAttack = false;
            }
        }  
    }

    public void StartHoldAttack(string input, string targetAnimation, int effect, int failDamage){
        targetInput = input;
        currentEffect = effect;
        critDamage = -1;
        waitingForInput = true;
        inHoldAttack = true;
        this.failDamage = failDamage;
        this.targetAnimation = targetAnimation;
        animPlay.Stop(false);
    }

    public void HoldAttackAcitvate(){
        attackReady = true;
    }

    public void HoldAttackPefectStart(int critDamage){
        this.critDamage = critDamage;
    }

    public void HoldAttackPerfectStop(){
        critDamage = -1;
    }

    public void HoldAttackTimeout(int failDamage){
        //DealDamage(failDamage, targets[currentEffect], aligns[currentEffect]);
    }

    public void WaitForInput(string input){
        animPlay.Stop(false);
        targetInput = input;
    }

    public override void DealDamage(int effectNum){
        Targeting damageTargets = targets[effectNum];
        int dmg = effectValue[effectNum];
        AbilityAlignment damageType = aligns[effectNum];
        
        foreach(Targeting target in Enum.GetValues(typeof(Targeting))){
            if((target & damageTargets) == target){
                
            }
        }
    }

    public void SpawnNode(string path){
        PackedScene scene = (PackedScene) GD.Load(path);
        Node effect = scene.Instance();
        AddChild(effect);
    }

    public void ExecuteEffect(int effectNum){
        switch(effects[effectNum]){
            case EffectType.Damage:
                break;
            case EffectType.Status:
                break;
            case EffectType.Healing:
                break;
        }
    }
}

