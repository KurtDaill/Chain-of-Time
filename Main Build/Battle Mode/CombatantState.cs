using System;
using Godot;

public abstract class CombatantState{
    public abstract CombatantState Process(Combatant player);

    public virtual void Enter(Combatant player, CombatantState lastState){ //Called by the dodge player when a new state is set
        return;
    }

    public virtual void HandleAnimationTransition(Combatant player){
        return;
    }

    public virtual void Exit(){
        return;
    }
}