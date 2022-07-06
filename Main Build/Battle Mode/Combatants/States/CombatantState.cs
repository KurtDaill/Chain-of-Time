using Godot;
using System;

public abstract class CombatantState{
    public abstract CombatantState Process(Combatant combatant, float delta);
    public virtual void Enter(Combatant combatant, CombatantState lastState){
        return;
    }

    public virtual void Exit(Combatant combatant){
        return;
    }
}

