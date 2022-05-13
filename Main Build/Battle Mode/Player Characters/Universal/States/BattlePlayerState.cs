using Godot;
using System;

//TODO State Machine Renaming Refactoring to be technically usable between enemies and players?
public abstract class PlayerCombatantState{
    public abstract PlayerCombatantState Process(Combatant player);

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

