using System;
using Godot;
using static AbilityUtilities;

public class SkillStateDebug : PlayerCombatantSkillState{
    public override CombatantState Process(Combatant player, float delta){
        GD.Print("Debug State Processed, is this a bug?");
        return null;
    }

    public SkillStateDebug(string newName, string newRulesText, int newCost, PlayerAbilityType newType){
        name = newName;
        rulesText = newRulesText;
        cost = newCost;
        type = newType;
    }
}