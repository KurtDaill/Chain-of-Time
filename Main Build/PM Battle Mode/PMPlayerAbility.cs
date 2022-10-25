using Godot;
using System;
using System.Collections.Generic;
using static PMBattleUtilities;

public class PMPlayerAbility : PMBattleAbility
{

    protected bool waitingForInput;
    protected string targetInput, targetAnimation, reverseAnimation;
    protected bool inHoldAttack, attackReady, inFluffHold, inReverse, readyForRelease;
    protected int delayCounter = 0;

    [Export(PropertyHint.MultilineText)] //Developers can specify the font size for their rules text within the rules text string using [textSize] and "small" or "smallest"
    protected string rulesText = "[textSize]<Insert Rules Text Here>";
    [Export]
    protected string abilityType = "Attack";
    [Export]
    protected int spCost = -1;

    public override void _Process(float delta)
    {
        if(delayCounter > 0){
            delayCounter--;
            return;
        }
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
        if(inReverse){
            if(Input.IsActionPressed(targetInput)){
                RestartFluffHold();
            }
        }else if (inFluffHold){
            if(!Input.IsActionPressed(targetInput)){
                if(readyForRelease){
                    animPlay.Play(targetAnimation);
                    inFluffHold = false;
                }else{
                    BackSlideFluffHold();
                }
            }
        }
    }

    /*
        StartHoldAttack begins a Hold Attack, where the player is intended to hold a button and release it in time.
        This function starts that process, and sets some basic parameters of it.
        Other methods (HoldAttackActivate, HoldAttackPerfectStart, etc.) should be called when appropriate on the Call Method Track. 
    */
    public void StartHoldAttack(string input, string targetAnimation, int failDamage){
        StartDelay(10);
        WaitForInput(input);
        critDamage = -1;
        inHoldAttack = true;
        this.failDamage = failDamage;
        this.targetAnimation = targetAnimation;
    }

    //For game feel reasons, we have some attacks that require the player to hold a key, but without mechanical challenge
    public void StartFluffHold(string input, string targetAnimation, string reverseAnimaiton){
        StartDelay(10);
        WaitForInput(input);
        readyForRelease = false;
        inFluffHold = true;
        this.targetAnimation = targetAnimation;
        this.reverseAnimation = reverseAnimaiton;
    }

    public void RestartFluffHold(){
        var targetSeek = animPlay.CurrentAnimationPosition;
        inReverse = false;
        animPlay.Play(targetAnimation);
        animPlay.Seek(targetSeek);
    }

    public void BackSlideFluffHold(){
        var targetSeek = animPlay.CurrentAnimationPosition;
        inReverse = true;
        animPlay.PlayBackwards(reverseAnimation);
        animPlay.Seek(targetSeek);
    }

    public void ReleaseReady(){
        readyForRelease = true;
    }
    public void StartDelay(int delay){
        delayCounter = delay;
    }

    //This method is called when the player has held the hold input for long enough for the ability to be allowed to go off
    public void HoldAttackActivate(){
        attackReady = true;
        failDamage = -1;
    }

    //This method is called when the player has held the hold input for long enough to be in the "sweet spot" range 
    public void HoldAttackPefectStart(int critDamage){
        this.critDamage = critDamage;
    }

    //This method is called when the player has held the hold input for long enough to fall out of "sweet spot" range 
    public void HoldAttackPerfectStop(){
        critDamage = -1;
    }
    //This method is called wwhen the player has held the hold input to 'time out', automatically going to the target animation and dealing failDamage
    public void HoldAttackTimeout(int failDamage){
        this.failDamage = failDamage;
        animPlay.Play(targetAnimation);
        inHoldAttack = false;
    }

    public void WaitForInput(string input){
        animPlay.Stop(false);
        targetInput = input;
        waitingForInput = true;
    }

    public int GetSPCost(){
        return spCost;
    }

    public string GetRulesText(){
        return rulesText;
    }

    public string GetAbilityType(){
        return abilityType;
    }
}

