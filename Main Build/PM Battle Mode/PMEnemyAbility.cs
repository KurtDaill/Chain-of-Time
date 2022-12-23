using Godot;
using System;
using System.Collections.Generic;
using static BattleEnemyAI;
public class PMEnemyAbility : PMBattleAbility
{
    [Export(PropertyHint.Enum)]
    protected List<TargetPriority> targetsInPriorityOrder = new List<TargetPriority>(){TargetPriority.MeleeHero};
    
    [Export(PropertyHint.Enum)]
    protected List<SpecialRequirement> requirements = new List<SpecialRequirement>();


    //This variable is used to store whether we're within the window of time when the player can actively defend
    private bool playerDefenseOnline = false;
    //This variable is flipped to true if the player defended too early and has lost their opertunity
    private bool defenseLocked = false;
    public override void _Ready()
    {
        base._Ready();
    }

    public List<SpecialRequirement> GetRequirements(){
        return requirements;
    }

    public List<TargetPriority> GetTargetingPriorities(){
        return targetsInPriorityOrder;
    }

    public override void _Process(float delta){
        base._Process(delta);
        if(Input.IsActionJustPressed("com_def") && !defenseLocked){
            if(playerDefenseOnline){
                successfulDefense = true;
                foreach(PMPlayerCharacter defender in target){
                    defender.PlayDefenseAnimation();
                }
            }else{
                defenseLocked = true;
            }
        }
    }

    public override void ExecuteEvent(int eventNum)
    {
        base.ExecuteEvent(eventNum);
    }

    public void PlayerDefenseReactionOnline(){
        playerDefenseOnline = true;
    }
    public void DisablePlayerDefense(){
        playerDefenseOnline = false;
    }
    public void StartAttackRun(){
        playerDefenseOnline = false;
        successfulDefense = false;
        defenseLocked = false;
    }

    public void ChargeUp(){
        var en = (PMEnemyCharacter) source;
        en.chargedUp = true;
    }

    public void ExpendCharge(){
        var en = (PMEnemyCharacter) source;
        en.chargedUp = false;
    }

    public override void FinishSequence()
    {
        base.FinishSequence();
        var en = (PMEnemyCharacter) source;
        if(en.chargedUp){
            en.animPlay.Play("ChargedIdle");
        }
    }
}

public static class BattleEnemyAI{
    public enum TargetPriority{
        Self,
        MeleeHero,
        MeleeEnemy,
        RangedHeroes,
        RangedEnemies,
        HeroDamageLeader,
        HeroHealingLeader,
        HeroBuffLeader,
        HeroTanking,
        EnemyMinion = 0,
        EnemyTank = 1,
        EnemyBruiser = 2,
        EnemyArtillery = 3,
        EnemySupport = 4,
        EnemySquadLeader = 5,
        EnemyBoss = 6,
        AnyHero,
        AnyEnemy
    }

    public enum SpecialRequirement{
        NoHeroTank,
        HeroDown,
        InFrontLine,
        SelfDamagedThisTurn,
        SelfUndamagedThisTurn,
        NoEnemies,
        ThreeEnemies,
        SelfHurt,
        EnemyHurt,
        SelfBloodied,
        EnemyBloodied,
        Charged
    }
}
