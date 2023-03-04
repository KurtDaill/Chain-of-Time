using Godot;
using System;

public partial class ExploreNPC : Node3D
{
	[Export]
	public bool hasCutscene;
	[Export]
	public bool cutsceneRepeats;
	[Export]
	public Node3D cutsceneNode;
	private CutsceneDirector cutscene;
	[Export]
	public CutsceneType cutsceneType;
	[Export]
	public Area3D interactArea;

	[Export]
	public bool enabled = true;
	[Export]
	private string storyFlagRequiredForCutscene = "";

	private bool armed = false;

	public enum CutsceneType{
		Talk,
		Listen
	}

	public override void _Ready(){
		if(!this.GetGroups().Contains("NPC")){
			this.AddToGroup("NPC");
		}
		if(!this.interactArea.GetGroups().Contains("NPC")){
			this.interactArea.AddToGroup("NPC");
		}
		cutscene = (CutsceneDirector) cutsceneNode;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public bool ArmCutscene(){
		if(hasCutscene){	
			armed = true;
			return true;
		}
		return false;
	}

	public void DisarmCutscene(){
		armed = false;
	}

	public void PlayCutscene(){
		if(enabled ){
			if(storyFlagRequiredForCutscene != "" && !GetNode<GameMaster>("/root/GameMaster").GetFlagValue(storyFlagRequiredForCutscene)) return;
			cutscene.StartCutscene();
			if(!cutsceneRepeats) hasCutscene = false;
			this.DisarmCutscene();
		}
	}
}
