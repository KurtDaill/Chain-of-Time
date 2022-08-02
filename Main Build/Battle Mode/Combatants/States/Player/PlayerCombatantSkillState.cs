using System;
using Godot;
using System.Collections.Generic;
using static AbilityUtilities;

public abstract class PlayerCombatantSkillState : CombatantAbilityState{
    protected string name, rulesText;
    protected int cost;
    protected PlayerAbilityQualities playerQualities;

    public string GetAbilityName(){
        return name;
    }

    public string GetRulesText(){
        return rulesText;
    }

    public int GetCost(){
        return cost;
    }
    public PlayerAbilityQualities GetAbilityType(){
        return playerQualities;
    }

    public Dictionary<String, object> Save(){
        return new Dictionary<string, object>{
            {"type", this.GetType()},
            {"name", name},
            {"cost", cost},
            {"playerQualities", playerQualities}
        };
    }

    //public Dictionary<String, object> Load()   
}