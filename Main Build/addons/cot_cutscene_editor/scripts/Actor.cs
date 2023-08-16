using Godot;
using System;
using System.Threading.Tasks;
[Tool]
public partial class Actor : Node3D
{
	[Export]
	protected string actorName;
	[Export]
	protected double walkingSpeedUnitsPerSecond = 2;
	[Export]
	protected Color characterColor;
	[Export]
	protected AudioStreamPlayer OneSyllablePip;
	[Export]
	protected AudioStreamPlayer TwoSyllablePip;
	[Export]
	protected AudioStreamPlayer EmphasisPip;

	protected double blockingTimeElapsed, blockingTimeTotal;
	string destinationBlockingMarkerName;
	protected CutsceneDialogueBox dialogueBox;
	protected AnimationPlayer animPlay;
	// Called when the node enters the scene tree for the first time.

	[Signal]
	public delegate void CompletedBlockingMovementEventHandler(string destinationMarker);

	bool moving = false;
	Vector3 blockingTargetGlobalPosition, blockingOriginGlobalPosition;
	public override void _Ready()
	{
		//dialogueBox = this.GetNode<CutsceneDialogueBox>("Dialogue Box");
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");	
	}

	public string GetActorName(){
		return actorName;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(moving){
			blockingTimeElapsed += delta;
			if(blockingTimeElapsed > blockingTimeTotal){
				this.GlobalPosition = blockingTargetGlobalPosition;
				moving = false;
				animPlay.Play("Idle");
				EmitSignal(Actor.SignalName.CompletedBlockingMovement, destinationBlockingMarkerName);
			}
			else this.GlobalPosition = blockingOriginGlobalPosition.Lerp(blockingTargetGlobalPosition, (float)(blockingTimeElapsed/blockingTimeTotal));
		}
	}

	public AnimationPlayer GetAnimationPlayer(){
		return animPlay;
	}
	public CutsceneDialogueBox GetDialogueBox(){
		return dialogueBox;
	}

	public Color GetColor(){
		return characterColor;
	}

	public void StartBlockingMovement(Vector3 target, string markerName){
		blockingOriginGlobalPosition = this.GlobalPosition;
		blockingTargetGlobalPosition = target;
		moving = true;
		animPlay.Play("Walk"); //TODO add in walks to different directions
		//Calculate interpolation rate:
		//Basically "Time it takes Actor to Walk somewhere = Lenght between the original and target position divided by walking speed
		blockingTimeTotal = Math.Abs((blockingTargetGlobalPosition - blockingOriginGlobalPosition).Length())/walkingSpeedUnitsPerSecond;

		blockingTimeElapsed = 0;
		destinationBlockingMarkerName = markerName;
	}
}
