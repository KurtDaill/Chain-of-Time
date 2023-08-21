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
			animPlay.Play("Show Listen");
		}else{
			animPlay.Play("Show Talk");
		}
	}

	public void HidePrompt(){
		animPlay.Play("Hide Prompt");
	}
}
