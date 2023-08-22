using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public partial class QuestMaster : Node
{
	Node possibleQuests;
	Node inProgressQuests;
	Node completedQuests;
	Node turnedInQuests;
	// Called when the node enters the scene tree for the first time.
	[Signal]
	public delegate void QuestsUpdatedEventHandler(Quest[] activeQuests);
	public override void _Ready()
	{
		possibleQuests = GetNode("Possible");
		inProgressQuests = GetNode("In Progress");
		completedQuests = GetNode("Completed");
		turnedInQuests = GetNode("Turned In");
		GetNode<GameMaster>("/root/GameMaster").GameModeBegin += ConnectToNewGameMode;
		foreach(Quest quest in inProgressQuests.GetChildren().Where(x => x is Quest)){
			quest.QuestComplete += RespondToQuestUpdates;
			foreach(QuestObjective obj in quest.GetNode("Objectives").GetChildren().Where(x => x is QuestObjective)){
				obj.ObjectiveComplete += RespondToQuestUpdates;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//TODO Create a Function to Handle New Quests Hitting inProgress

	public void ConnectToNewGameMode(GameplayMode newMode)
	{
		foreach(Quest quest in inProgressQuests.GetChildren().Where(x => x is Quest)){
			quest.ConnectToGameMode(newMode);
		}
	}

	public void RespondToQuestUpdates(){
		EmitSignal(QuestMaster.SignalName.QuestsUpdated, GetAciveQuests());
	}

	public Quest[] GetAciveQuests(){
		List<Quest> quests = new List<Quest>();
		foreach(Node node in (inProgressQuests.GetChildren() + completedQuests.GetChildren()).Where(x => x is Quest)){
			quests.Add(node as Quest);
		}
		return quests.ToArray();
	}
}
