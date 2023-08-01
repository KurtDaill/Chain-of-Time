using Godot;
using System;

public partial class InteractionPoint : DialogueInteractable
{
	AnimationPlayer animPlay;
	[Export]
	private string verb;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
		this.GetNode<Label3D>("Label/Label3D").Text = verb;

		if(!this.GetGroups().Contains("Interaction Points")){
			this.AddToGroup("Interaction Points");
		}
		if(!this.interactArea.GetGroups().Contains("Interaction Points")){
			this.interactArea.AddToGroup("Interaction Points");
		}

		base._Ready();
	}

    public override bool ArmCutscene()
    {
		if(!enabled) return false;
		animPlay.Play("ShowPrompt");
        return base.ArmCutscene();
    }

	public override void DisarmCutscene(){
		if(armed)animPlay.Play("HidePrompt");
		base.DisarmCutscene();
	}

	public override async void PlayCutscene(){
		if(enabled && armed){
			if(storyFlagRequiredForCutscene != "" && !GetNode<GameMaster>("/root/GameMaster").GetFlagValue(storyFlagRequiredForCutscene)) return;
			
			//cutscene.StartCutscene();
			animPlay.Play("HidePrompt");
			await ToSignal(cutscene, "CutsceneCompleted");
			animPlay.Play("ShowPrompt");
			
			if(!cutsceneRepeats) hasCutscene = false;
			this.DisarmCutscene();
		}
	}
}
