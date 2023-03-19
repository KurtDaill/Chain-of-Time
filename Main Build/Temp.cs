using Godot;
using System;

public partial class Temp : Area3D
{
	[Export]
	CutsceneDirector cutscene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.AddToGroup("Temp");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnAreaEntered(){
		cutscene.StartCutscene();
	}
}

