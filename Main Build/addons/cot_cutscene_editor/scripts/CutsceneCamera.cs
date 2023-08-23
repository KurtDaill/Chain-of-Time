using Godot;
using System;
[Tool]
public partial class CutsceneCamera : Camera3D
{
	private bool moving;
	private double timeElapsed;
	private double transitionLength;
	private string transitionType;
	private ShotDetails targetShotDetails;
	private ShotDetails startingShotDetails;
	[Signal]
	public delegate void ShotTransitionCompleteEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(moving){
			//Actually move the camera;
			switch(transitionType){
				case "linear" :
					timeElapsed += delta;
					if(timeElapsed >= transitionLength) timeElapsed = transitionLength;
					this.GlobalTransform = startingShotDetails.GetGlobalTransform().InterpolateWith(targetShotDetails.GetGlobalTransform(), (float)(timeElapsed/transitionLength));
					this.Fov = (startingShotDetails.GetFieldOfView() * (1 - ((float)(timeElapsed/transitionLength)))) + (targetShotDetails.GetFieldOfView() * (float)(timeElapsed/transitionLength));
					if(timeElapsed == transitionLength){
						EmitSignal(CutsceneCamera.SignalName.ShotTransitionComplete);
						moving = false;
					}
					break;
				default:
					throw new ArgumentException("TransitionType : " + transitionType + " not recognized by CutsceneCamera Object!");
			}
		}
	}

	//Returns whether or not the director should wait for a signal from this object to continue
	public bool StartTransition(ShotDetails shotDetails, string transitionType, double transitionLength = 0){
		switch(transitionType){
			case "cut" :
				this.GlobalTransform = shotDetails.GetGlobalTransform();
				this.Fov = shotDetails.GetFieldOfView();
				this.Size = shotDetails.GetSize();
				return false;
			case "linear" :
				ConfigureTransitionInformationForMovement(shotDetails, transitionType, transitionLength);
				return true;
			default:
				throw new ArgumentException("TransitionType : " + transitionType + " not recognized by CutsceneCamera Object!");
		}
	}

	private void ConfigureTransitionInformationForMovement(ShotDetails shotDetails, string transitionType, double transitionLength = 0){
		this.transitionType = transitionType;
		this.transitionLength = transitionLength;
		targetShotDetails = shotDetails;

		startingShotDetails = new ShotDetails(this.GlobalTransform, this.Fov, this.Size);

		timeElapsed = 0;
		moving = true;
	}
}
