using Godot;
using System;

public abstract class EnemyCombatant : Combatant{
    [Export]
    public NodePath AnimationTree;
    public CombatantAbilityState[] abilitiesKnown;

    public CombatantAbilityState currentAbility = null;
    
    public virtual bool DodgeBehaviour(float delta){
        var newState = state.Process(this, delta);
        if(newState != null){
            SetState(newState);
        }
        return (data.GetBool("painState") && IsOnFloor());
    }

    public abstract void DecideAbility();
}