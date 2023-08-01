using Godot;
using System;

public partial class TimeFragment : Area3D
{
	protected AnimationPlayer animPlay;

	protected bool armed = false;

	protected bool isPortalBack;

	protected bool armedForReturn = false;

	[Export]
	protected ChronoEnviro timeEnvironment;
	[Export]
	protected CronoScene targetCronoScene;
	[Export(PropertyHint.File)]
	private string targetEnvironmentRes;
	private Godot.Environment targetEnvironment;
	
	[Export]
	protected CutsceneDirector postTravelCustcene = null;

	protected bool cutsceneAlreadyPlayed = false;

	[Export]
	private DirectionalLight3D targetSun;

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
	public virtual bool ArmTimeFragment(){
		if(timeEnvironment.IsInPast()){
			//if(isPortalBack){
				timeEnvironment.SetReturnTimeFragment(this);
				armedForReturn = true;
				animPlay.Play("Show Return Prompt");
				return true;
			//}else return false;
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

	public virtual void TimeTravel(ExplorePlayer caller){
		if(armed){
			timeEnvironment.DoTheTimeWarp(this, caller);
			this.isPortalBack = true;
		}
		else if(armedForReturn) timeEnvironment.ReturnToThePresent();
		
	}

	public virtual CronoScene GetTargetCronoScene(){
		return targetCronoScene;
	}

	public virtual Godot.Environment GetTargetEnvironment(){
		return (Godot.Environment)targetEnvironment;
	}

	public virtual DirectionalLight3D GetTargetSun(){
		return (DirectionalLight3D)targetSun;
	}

	public bool HasCutsceneArmed(){
		return (postTravelCustcene != null && !cutsceneAlreadyPlayed);
	}

	public void PlayCutscene(){
		cutsceneAlreadyPlayed = true;
		//postTravelCustcene.StartCutscene();
	}
}
