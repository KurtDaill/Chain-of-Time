using Godot;
using System;
using System.Linq;
public partial class QuestMaster : Node
{
	Node possibleQuests;
	Node inProgressQuests;
	Node completedQuests;
	Node turnedInQuests;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		possibleQuests = GetNode("Possible");
		inProgressQuests = GetNode("In Progress");
		completedQuests = GetNode("Completed");
		turnedInQuests = GetNode("Turned In");
		GetNode<GameMaster>("/root/GameMaster").GameModeBegin += ConnectToNewGameMode;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ConnectToNewGameMode(GameplayMode newMode)
	{
		foreach(Quest quest in inProgressQuests.GetChildren().Where(x => x is Quest)){
			quest.ConnectToGameMode(newMode);
		}
	}
}
