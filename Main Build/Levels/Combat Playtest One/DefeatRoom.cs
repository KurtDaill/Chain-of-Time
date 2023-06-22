using Godot;
using System;

public partial class DefeatRoom : Node3D
{
	
	[Export(PropertyHint.File)]
	private string safeRoomPath;
	PackedScene safeRoom;

	public override void _Ready(){
		safeRoom = GD.Load<PackedScene>(safeRoomPath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("ui_start")){
			GetTree().ChangeSceneToPacked(safeRoom);
		}else if(Input.IsActionJustPressed("ui_back")){
			GetTree().Quit();
		}
	}
}
