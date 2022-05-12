using Godot;
using System;

public abstract class PlayerCombatantState {
    public abstract PlayerCombatantState Process(PlayerCombatant player);

    public virtual void Enter(PlayerCombatant player, PlayerCombatantState lastState){ //Called by the dodge player when a new state is set
        return;
    }

    public virtual void HandleAnimationTransition(PlayerCombatant player){
        return;
    }

    public virtual void Exit(){
        return;
    }
}

