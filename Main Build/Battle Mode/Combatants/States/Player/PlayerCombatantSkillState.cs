using System;
using Godot;
using static AbilityUtilities;

public abstract class PlayerCombatantSkillState : CombatantAbilityState{
    protected string name, rulesText;
    protected int cost;
    protected PlayerAbilityType type;

    public string GetAbilityName(){
        return name;
    }

    public string GetRulesText(){
        return rulesText;
    }

    public int GetCost(){
        return cost;
    }
    public PlayerAbilityType GetAbilityType(){
        return type;
    }
}