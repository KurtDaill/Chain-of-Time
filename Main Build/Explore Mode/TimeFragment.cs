using Godot;
using System;

public partial class TimeFragment : Area3D
{
	private AnimationPlayer animPlay;

	private bool armed = false;

	private bool isPortalBack;

	private bool armedForReturn = false;

	[Export]
	ChronoEnviro timeEnvironment;
	[Export]
	CronoScene targetCronoScene;
	[Export(PropertyHint.File)]
	string targetEnvironmentRes;
	Godot.Environment targetEnvironment;
	
	[Export]
	CutsceneDirector postTravelCustcene = null;

	bool cutsceneAlreadyPlayed = false;

	[Export]
	DirectionalLight3D targetSun;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
		targetEnvironment = GD.Load<Godot.Environment>(targetEnvironmentRes);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	//Returns whether this Fragment was armed;
	public bool ArmTimeFragment(){
		if(timeEnvironment.IsInPast()){
			if(isPortalBack){
				armedForReturn = true;
				animPlay.Play("Show Return Prompt");
				return true;
			}else return false;
		}else{
			armed = true;
			animPlay.Play("Show Prompt");
			return true;
		}
	}

	//Returns whether this Fragment was disarmed;
	public bool DisarmTimeFragment(){
		if(armed){
			armed = false;
			animPlay.Play("Hide Prompt");
			return true;
		}
		if(armedForReturn){
			armedForReturn = false;
			animPlay.Play("Hide Return Prompt");
			return true;
		}
		else{
			return false;
		}
	}

	public void TimeTravel(ExplorePlayer caller){
		if(armed){
			timeEnvironment.DoTheTimeWarp(this, caller);
			this.isPortalBack = true;
		}
		else if(armedForReturn) timeEnvironment.ReturnToThePresent();
		
	}

	public CronoScene GetTargetCronoScene(){
		return targetCronoScene;
	}

	public Godot.Environment GetTargetEnvironment(){
		return targetEnvironment;
	}

	public DirectionalLight3D GetTargetSun(){
		return targetSun;
	}

	public bool HasCutsceneArmed(){
		return (postTravelCustcene != null && !cutsceneAlreadyPlayed);
	}

	public void PlayCutscene(){
		cutsceneAlreadyPlayed = true;
		postTravelCustcene.StartCutscene();
	}
}
