using Godot;
using System;

public abstract partial class DialogueInteractable : Node3D{
	[Export]
	public bool hasCutscene;
	[Export]
	public bool cutsceneRepeats;
	[Export]
	public Node cutsceneNode;
	protected CutsceneDirector cutscene;
	[Export]
	public Area3D interactArea;

	protected bool armed = false;

	[Export]
	public bool enabled = true;
	[Export]
	protected string storyFlagRequiredForCutscene = "";

    public override void _Ready(){
        cutscene = (CutsceneDirector) cutsceneNode;
    }

	public virtual bool ArmCutscene(){
		if(hasCutscene){
			armed = true;
			return true;
		}
		return false;
	}

	public virtual void DisarmCutscene(){
		armed = false;
	}

    public virtual async void PlayCutscene(){
		if(enabled){
			if(storyFlagRequiredForCutscene != "" && !GetNode<GameMaster>("/root/GameMaster").GetFlagValue(storyFlagRequiredForCutscene)) return;
				cutscene.StartCutscene();
				await ToSignal(cutscene, "CutsceneCompleted");
			if(!cutsceneRepeats) hasCutscene = false;
				this.DisarmCutscene();
		}
	}
}