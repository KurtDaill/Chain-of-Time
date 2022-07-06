using Godot;
using System;


//Class used to represent and run turn based battles
public abstract class BattleCommand
{
    protected Battle parent;

    protected bool runningDual = false; //Is this command being dual run with the command immediately ahead of it
    public virtual void Enter(Battle parent, bool dual = false){
        this.parent = parent;
        runningDual = dual;
        //Called when this command is picked at the current command.
    }

    public void DisableDualRunning(){
        runningDual = false; 
    }
    public abstract void Execute(float delta, Battle parent);

    public abstract void Undo();

    public virtual void Exit(){
        if(runningDual){
            parent.HandleDualExit();
        }
        //Called when the battle is moving to the next command.
        //Used to log any needed values or execute final behaviours.
    }
}

/*
Remarks:

*/