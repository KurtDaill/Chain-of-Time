using Godot;
using System;

public partial class LevelTransition : Area3D
{
	// Called when the node enters the scene tree for the first time.
	[Export(PropertyHint.File)]
	string gotoScenePath;
	public override void _Ready()
	{
		this.AddToGroup("Transition");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Transition(){
		PackedScene nextScene = GD.Load<PackedScene>(gotoScenePath);
		GetTree().ChangeSceneToPacked(nextScene);
	}
}
