using Godot;
using System;
public class PlayerCombatant : Combatant
{
    public virtual void Move(float delta)
    {
        if(state == null){
            throw new ArgumentNullException();
        }
        
        newState = state.Process(this, delta);
        if(newState != null) SetState(newState);
        newState = null;
    }

    public virtual bool MoveAndAttack(EnemyCombatant[] targets, int[] damageRecord, float delta){
        return false;
    } //TODO Refactor
}