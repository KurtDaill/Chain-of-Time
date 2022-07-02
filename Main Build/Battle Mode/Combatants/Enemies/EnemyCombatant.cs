using Godot;
using System;

public class EnemyCombatant : Combatant{
    public virtual bool DodgeBehaviour(){
        var newState = state.Process(this);
        if(newState != null){
            SetState(newState);
        }
        return (data.GetBool("painState") && IsOnFloor());
    }
}