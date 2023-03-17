using Godot;
using System;

public partial class ExploreNPC : DialogueInteractable
{
	[Export]
	private Node dialgouePromptNode;
	private DialoguePrompt dialoguePrompt;

	private int currentPromenade;
	
	[Export]
	public CutsceneType cutsceneType;
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

	public override bool ArmCutscene(){
		if(hasCutscene){
			ShowDialoguePrompt();	
			armed = true;
			return true;
		}
		return false;
	}

	public override void DisarmCutscene(){
		armed = false;
		HideDialoguePrompt();
	}

	public override async void PlayCutscene(){
		if(enabled){
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
