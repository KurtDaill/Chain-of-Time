using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static PMBattleUtilities;

public class PMPlayerAbility : PMBattleAbility
{

    protected bool waitingForInput;
    protected string targetInput, targetAnimation, mainAnimation, reverseAnimation;
    protected string critAnimation = "";
    protected string failAnimation = "";
    protected bool inHoldAttack, attackReady, inFluffHold, inReverse, readyForRelease;
    protected int delayCounter = 0;

    [Export(PropertyHint.MultilineText)] //Developers can specify the font size for their rules text within the rules text string using [textSize] and "small" or "smallest"
    protected string rulesText = "[textSize]<Insert Rules Text Here>[damageNumber]";
     //This is the damage value displayed in rules text, and is the figure that's modified by bonuses/penalties (where [damageNumber] is written) before being shown to the player,
     //leave at -1 if you don't want to display an ammount of damage this attack deals
    [Export]
    protected int listedDamage = -1;
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
                if(critAnimation != "") animPlay.Play(critAnimation);
                else animPlay.Play(targetAnimation);
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
    public void StartHoldAttack(string input, string targetAnimation, int failDamage, string failAnim = ""){
        StartDelay(10);
        WaitForInput(input);
        critDamage = -1;
        inHoldAttack = true;
        this.failDamage = failDamage;
        this.targetAnimation = targetAnimation;
        mainAnimation = animPlay.CurrentAnimation;
        critAnimation = ""; //TODO: Make this cleaner
        failAnimation = failAnim;
    }

    //For game feel reasons, we have some attacks that require the player to hold a key, but without mechanical challenge
    public void StartFluffHold(string input, string targetAnimation, string reverseAnimaiton){
        StartDelay(10);
        WaitForInput(input);
        readyForRelease = false;
        inFluffHold = true;
        mainAnimation = animPlay.AssignedAnimation;
        this.targetAnimation = targetAnimation;
        this.reverseAnimation = reverseAnimaiton;
    }

    public void RestartFluffHold(){
        var targetSeek = animPlay.CurrentAnimationPosition;
        inReverse = false;
        animPlay.Play(mainAnimation);
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
        failAnimation = "";
    }

    //This method is called when the player has held the hold input for long enough to be in the "sweet spot" range 
    public void HoldAttackPefectStart(int critDamage, string critAnim = ""){
        this.critDamage = critDamage; 
        critAnimation = critAnim;
    }

    //This method is called when the player has held the hold input for long enough to fall out of "sweet spot" range 
    public void HoldAttackPerfectStop(){
        critDamage = -1;
        critAnimation = "";
    }
    //This method is called wwhen the player has held the hold input to 'time out', automatically going to the target animation and dealing failDamage
    public void HoldAttackTimeout(int failDamage, string failAnim = ""){
        this.failDamage = failDamage;
        animPlay.Play(targetAnimation);
        inHoldAttack = false;
        failAnimation = failAnim;
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
        var modifiedDamage = listedDamage;
        var print = rulesText;
        if(listedDamage != -1){
            if(source.GetMyStatuses().Contains(StatusEffect.Empowered) && usesEmpowered){
                modifiedDamage += source.statusEffects.Where<PMStatus>(x => x.GetStatusType() == StatusEffect.Empowered).ToArray<PMStatus>()[0].GetMagnitude();
            }
            if(source.GetMyStatuses().Contains(StatusEffect.Overcharged) && consumesOvercharged){
                modifiedDamage += source.statusEffects.Where<PMStatus>(x => x.GetStatusType() == StatusEffect.Overcharged).ToArray<PMStatus>()[0].GetMagnitude();
            }
            if(modifiedDamage > listedDamage){
                print = print.Replace("[damageNumber]", "[color=green]" + modifiedDamage + "[/color]");
            }else if(modifiedDamage < listedDamage){
                print = print.Replace("[damageNumber]", "[color=red]" + modifiedDamage + "[/color]");
            }else{
                print = print.Replace("[damageNumber]", "" + listedDamage);//We know modified damage = listed damage
            }
        }
        return print;
    }

    public string GetAbilityType(){
        return abilityType;
    }
}

