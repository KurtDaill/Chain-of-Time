using Godot;
using System;

public abstract class BattlePlayerState {
    public abstract BattlePlayerState Process(BattlePlayer player);

    public virtual void Enter(BattlePlayer player, BattlePlayerState lastState){ //Called by the dodge player when a new state is set
        return;
    }

    public virtual void HandleAnimationTransition(BattlePlayer player){
        return;
    }
}

