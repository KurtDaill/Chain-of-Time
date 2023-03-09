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
	[Export]
	private Node dialgouePromptNode;
	private DialoguePrompt dialoguePrompt;

	private bool armed = false;

	private int currentPromenade;
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
		dialoguePrompt = (DialoguePrompt) dialgouePromptNode;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public bool ArmCutscene(){
		if(hasCutscene){
			ShowDialoguePrompt();	
			armed = true;
			return true;
		}
		return false;
	}

	public void DisarmCutscene(){
		armed = false;
		HideDialoguePrompt();
	}

	public async void PlayCutscene(){
		if(enabled ){
			if(storyFlagRequiredForCutscene != "" && !GetNode<GameMaster>("/root/GameMaster").GetFlagValue(storyFlagRequiredForCutscene)) return;
				cutscene.StartCutscene();
				dialoguePrompt.Visible = false;
				await ToSignal(cutscene, "CutsceneCompleted");
				dialoguePrompt.Visible = true;
			if(!cutsceneRepeats) hasCutscene = false;
				this.DisarmCutscene();
		}
	}

	private void ShowDialoguePrompt(){
		switch(cutsceneType){
			case CutsceneType.Talk :
				dialoguePrompt.GetAnimPlay().Play("ShowTalk");
				break;
			case CutsceneType.Listen:
				dialoguePrompt.GetAnimPlay().Play("ShowListen");
				break;
		}
	}

	private void HideDialoguePrompt(){
		this.GetNode<AnimationPlayer>("DialoguePrompt/AnimationPlayer").Play("HidePrompt");
	}
}
