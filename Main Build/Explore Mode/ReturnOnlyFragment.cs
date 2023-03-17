using Godot;
using System;

public partial class ReturnOnlyFragment : TimeFragment
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		isPortalBack = true;
		base._Ready();
		timeEnvironment.LogReturnFragment(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//if(timeEnvironment.IsInPast()) this.Visible = true;
		//else this.Visible = false;	
	}

	public override bool ArmTimeFragment(){
		if(timeEnvironment.IsInPast()){
			if(isPortalBack){
				armedForReturn = true;
				animPlay.Play("Show Return Prompt");
				return true;
			}else return false;
		}
		return false;
	}

	public override void TimeTravel(ExplorePlayer caller){
		if(armedForReturn){
			timeEnvironment.SetReturnTimeFragment(this);
			timeEnvironment.ReturnToThePresent();
		}
	}

    public override CronoScene GetTargetCronoScene()
    {
        return null;
    }

	public override Godot.Environment GetTargetEnvironment(){
		return null;
	}

	public override DirectionalLight3D GetTargetSun(){
		return null;
	}


}
