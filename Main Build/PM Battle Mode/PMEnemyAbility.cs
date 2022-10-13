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

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public override void ExecuteEvent(int eventNum)
    {
        base.ExecuteEvent(eventNum);
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
        SelfDamagedThisTurn,
        SelfUndamagedThisTurn,
        NoEnemies,
        ThreeEnemies,
        SelfHurt,
        EnemyHurt,
        SelfBloodied,
        EnemyBloodied
    }
}
