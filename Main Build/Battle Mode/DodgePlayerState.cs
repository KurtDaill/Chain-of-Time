using Godot;
using System;

public abstract class DodgePlayerState {
    public abstract DodgePlayerState Process(DodgePlayer player);

    public virtual void Enter(DodgePlayer player){ //Called by the dodge player when a new state is set
        return;
    }

    public virtual void HandleAnimationTransition(DodgePlayer player){
        return;
    }
}

