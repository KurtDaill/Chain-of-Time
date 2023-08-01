using Godot;
using System;
using System.Threading.Tasks;
[Tool]
public partial class Actor : Node3D
{
	[Export]
	protected string actorName;
	protected CutsceneDialogueBox dialogueBox;
	protected AnimationPlayer animPlay;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dialogueBox = this.GetNode<CutsceneDialogueBox>("Dialogue Box");
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");	
	}

	public string GetActorName(){
		return actorName;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void SpeakLine(CutsceneLine line){
		dialogueBox.BeginDialogue(line);
	} 

	public AnimationPlayer GetAnimationPlayer(){
		return animPlay;
	}
	public CutsceneDialogueBox GetDialogueBox(){
		return dialogueBox;
	}
}
