using Godot;
using System;
using static GameplayUtilities;
public partial class PauseMenuGUI : CanvasLayer
{

	// Called when the node enters the scene tree for the first time.
	public override void _EnterTree()
	{

	}

	//returns true if we should switch back to the earlier gameplaymode
	public bool HandleInput(PlayerInput input){
		if(input == PlayerInput.Start){
			return true;	
		}	
		return false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _ExitTree()
	{

	}
}
