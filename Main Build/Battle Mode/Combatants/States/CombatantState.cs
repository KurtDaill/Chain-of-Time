using Godot;
using System;

public abstract class CombatantState{
    public abstract CombatantState Process(Combatant combatant);

    public virtual void Enter(Combatant combatant, CombatantState lastState){
        return;
    }

    public virtual void HandleAnimationTransition(Combatant combatant){
        return;
    }

    public virtual void Exit(){
        return;
    }
}

