using Godot;
using System;

public partial class QuestObjective : Node
{
    [Signal]
    public delegate void ObjectiveCompleteEventHandler();
    protected bool completed = false;
    [Export]
    protected string playerFacingDescription;
    
    public override void _Ready(){

    }

    public virtual void ConnectToSignalInMode(GameplayMode mode){
        return;
    }

    private void Complete(){
        if(!completed) EmitSignal(QuestObjective.SignalName.ObjectiveComplete);
        completed = true;
    }

    public bool IsCompleted(){
        return completed;
    }

    public string GetDescription(){
        return playerFacingDescription;
    }
}
