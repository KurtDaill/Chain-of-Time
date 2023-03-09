using Godot;
using System;

public partial class Waypoint : Node3D
{
	public int promenadeIndex;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetPromenade(int i){
		promenadeIndex = i;
	}

	public int GetPromenade(){
		return promenadeIndex;
	}
}
