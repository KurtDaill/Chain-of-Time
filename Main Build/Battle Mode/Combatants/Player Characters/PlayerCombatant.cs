using Godot;
using System;
public class PlayerCombatant : Combatant
{
    public virtual void Move()
    {
        if(state == null){
            throw new ArgumentNullException();
        }
        
        newState = state.Process(this);
        if(newState != null) SetState(newState);
        newState = null;
    }

    public virtual bool MoveAndAttack(EnemyCombatant[] targets, int[] damageRecord){
        return false;
    } //TODO Refactor
}