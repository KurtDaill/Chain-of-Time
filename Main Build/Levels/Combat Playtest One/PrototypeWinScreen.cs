using Godot;
using System;

public partial class PrototypeWinScreen : Node3D
{
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("ui_start")){
			GetTree().Quit();
		}
	}
}
