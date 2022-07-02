using Godot;
using System;


//Class used to represent and run turn based battles
public abstract class BattleCommand
{
    protected Battle parent;
    public virtual void Enter(Battle parent){
        this.parent = parent;
        //Called when this command is picked at the current command.
    }
    public abstract void Execute(float delta, Battle parent);

    public abstract void Undo();

    public virtual void Exit(){
        //Called when the battle is moving to the next command.
        //Used to log any needed values or execute final behaviours.
    }
}