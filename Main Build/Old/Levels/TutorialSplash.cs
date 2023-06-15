using Godot;
using System;

public partial class TutorialSplash : Node2D
{
	[Export(PropertyHint.File)]
	public string firstLevelPath;
	public PackedScene scene;
	private bool enabled = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scene = GD.Load<PackedScene>(firstLevelPath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _UnhandledInput(InputEvent @event){
		if(@event is InputEventJoypadButton eventAct){
			if(eventAct.Pressed && enabled){
				GetTree().ChangeSceneToPacked(scene);
				return;
			}
		}
		if(@event is InputEventKey eventKey){
			if(eventKey.Pressed && enabled){
				GetTree().ChangeSceneToPacked(scene);
				return;
			}
		}
	}

	public void Enable(){
		enabled = true;
	}
}
