using System;
using Godot;

public abstract class EnemyCombatantState {

    public abstract EnemyCombatantState Process(EnemyCombatant enemy);

    public virtual void Enter(EnemyCombatant enemy, EnemyCombatantState lastState){
        return;
    }

    public virtual void HandleAnimationTransition(EnemyCombatant enemy){
        return;
    }

    public virtual void Exit(){
        return;
    }
}