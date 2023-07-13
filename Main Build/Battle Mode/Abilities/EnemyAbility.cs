using System;
using Godot;

public partial class EnemyAbility : Ability
{
    public PlayerCombatant GetMeleeTarget(Battle battle){
        if(battle.GetRoster().GetCombatant(source.GetPosition().GetLane(), BattleUtilities.BattleRank.HeroFront) != null)
            return (PlayerCombatant)battle.GetRoster().GetCombatant(source.GetPosition().GetLane(), BattleUtilities.BattleRank.HeroFront);
        
        foreach(Combatant com in battle.GetRoster().GetCombatantsByRank(BattleUtilities.BattleRank.HeroFront)){
            return (PlayerCombatant) com;
        }
        throw new ArgumentException("Battle has no player character in the front rank...WHAT?!");
    }
}