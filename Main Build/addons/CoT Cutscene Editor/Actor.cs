using Godot;
using System;

[Tool]
public partial class Actor : Node3D
{
	[Export]
	protected string name;
	protected SpeechBubble speechBubble;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	public string GetName(){
		return name;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
