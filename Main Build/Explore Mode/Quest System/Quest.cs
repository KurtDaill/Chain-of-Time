using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Quest : Node
{
    [Export] //Whether the player needs to speak to an NPC to fully complete this quest
    bool RequiresTurnIn = false;
    [Signal]
    public delegate void QuestCompleteEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        foreach(QuestObjective obj in this.GetChildren().Where(x => x is QuestObjective)){
            obj.ObjectiveCompleted += OnObjectiveCompleted;
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public virtual void ConnectToGameMode(GameplayMode mode){
        switch(mode.GetType().Name){
            case nameof(CutsceneDirector):
                foreach(TalkToNPCQuestObjective talkObj in this.GetChildren().Where(x => x is TalkToNPCQuestObjective)){
                    talkObj.ConnectToSignalInMode(mode);
                }
                break;
        }
    }

    public void OnObjectiveCompleted(){
        List<QuestObjective> objectives = this.GetChildren().OfType<QuestObjective>().ToList();
        if(objectives.Count(x => !x.IsCompleted()) == 0){
            EmitSignal(Quest.SignalName.QuestComplete);
        }
    }
}
