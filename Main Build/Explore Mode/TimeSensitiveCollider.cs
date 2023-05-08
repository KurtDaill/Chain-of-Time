using Godot;
using System;

public partial class TimeSensitiveCollider : StaticBody3D
{
	[Export]
	private bool enabledInTimeScene = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetEnableInScene(bool set){
		enabledInTimeScene = set;
	}

	public void Enable(){
		foreach(Node node in this.FindChildren("", "CollisionShape3D")){
			((CollisionShape3D)node).Disabled = !(enabledInTimeScene);
		}
	}

	public void Disable(){
		foreach(Node node in this.FindChildren("", "CollisionShape3D")){
			((CollisionShape3D)node).Disabled = true;
		}

	}
}
