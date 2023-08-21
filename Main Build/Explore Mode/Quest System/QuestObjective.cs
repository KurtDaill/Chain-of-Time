using Godot;
using System;

public partial class QuestObjective : Node
{
    [Signal]
    public delegate void ObjectiveCompletedEventHandler();
    bool completed = false;
    
    public override void _Ready(){

    }

    public virtual void ConnectToSignalInMode(GameplayMode mode){
        return;
    }

    private void Complete(){
        if(!completed) EmitSignal(QuestObjective.SignalName.ObjectiveCompleted);
        completed = true;
    }

    public bool IsCompleted(){
        return completed;
    }
}
