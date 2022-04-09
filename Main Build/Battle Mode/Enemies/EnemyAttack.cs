using Godot;
using System;

public abstract class EnemyAttack
{
    public virtual void Enter(){ //Called when attack begins, does any required setup
        return;
    }

    public abstract bool Execute(); //Called every frame while attack is executing, handles frame by frame interactions
    //returns true unless the attack is completed, in which case it returns false;

    public virtual void Exit(){ //Called when attack ends, does any required cleanup
        return;
    }
}
