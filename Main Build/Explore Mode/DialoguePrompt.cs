using Godot;
using System;

public partial class DialoguePrompt : Node3D
{
	[Export]
	bool isListen = false;
	private AnimationPlayer animPlay;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public AnimationPlayer GetAnimPlay(){
		return animPlay;
	}

	public void ShowPrompt(){
		if(isListen){
			animPlay.Play("ShowListen");
		}else{
			animPlay.Play("ShowTalk");
		}
	}

	public void HidePrompt(){
		animPlay.Play("HidePrompt");
	}

	public void HidePromptForCutscene(CutsceneDirector cutscene){
		animPlay.Play("HidePrompt");
		animPlay.Seek(animPlay.GetAnimation("HidePrompt").Length);
		this.Visible = false;
		cutscene.CutsceneComplete += MakeSelfVisible;
	}

	private void MakeSelfVisible(string cutsceneName){
		this.Visible = true;
	}
}
