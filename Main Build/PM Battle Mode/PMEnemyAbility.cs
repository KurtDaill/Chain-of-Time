using Godot;
using System;
using System.Collections.Generic;
using static BattleEnemyAI;
public class PMEnemyAbility : PMBattleAbility
{
    [Export(PropertyHint.Enum)]
    protected List<TargetPriority> targetingLogic = new List<TargetPriority>();
    
    [Export(PropertyHint.Enum)]
    protected List<SpecialRequirement> requirements = new List<SpecialRequirement>();
   
    public override void _Ready()
    {
        
    }

    public List<SpecialRequirement> GetRequirements(){
        return requirements;
    }

    public List<TargetPriority> GetTargetingLogic(){
        return targetingLogic;
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
        MeleeHero,
        MeleeEnemy,
        RangedHeroes,
        RangedEnemies,
        BossEnemy,
        HeroDamageLeader,
        HeroHealingLeader,
        HeroBuffLeader,
        HeroTanking,
        EnemyTank
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
