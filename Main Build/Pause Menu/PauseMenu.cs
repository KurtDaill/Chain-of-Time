using Godot;
using System;
using System.Threading.Tasks;

public partial class PauseMenu : GameplayMode
{
	PauseMenuGUI gui;
	bool goBack = false;

	GameplayMode returnMode;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gui = this.GetChild(0) as PauseMenuGUI;
		gui.Visible = false;
        ProcessMode = ProcessModeEnum.WhenPaused;
	}

	public override Task StartUp(GameplayMode oldMode){
		//Add the pause menu attached to this node to the scene tree.
		//this.RemoveChild(gui);
		//GetTree().Root.AddChild(gui);
        GetTree().Paused = true;
		returnMode = oldMode;
		gui.Visible = true;
		gui.Open();
		return Task.CompletedTask;
	}

    public override async Task<GameplayMode> RemoteProcess(double delta)
    {
        if(goBack){
			goBack = false;
			return returnMode;
		}
		return null;
    }

    public override void HandleInput(GameplayUtilities.PlayerInput input)
    {
        goBack = gui.HandleInput(input);
    }

    public override Task TransitionOut()
    {
		//GetTree().Root.RemoveChild(gui);
		//this.AddChild(gui);
		gui.Visible = false;
		gui.Close();
		GetTree().Paused = false;
        return base.TransitionOut();
    }
}
