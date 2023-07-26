using Godot;
using System;

public partial class CutsceneDummy : Node3D
{
	[Export(PropertyHint.File)]
	string scriptXML;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenPlayLoader.LoadScript(scriptXML);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
