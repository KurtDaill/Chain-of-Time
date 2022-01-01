using Godot;
using System;

public class Bullet : Area2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.

	[Export]
	public float xBulletSpeed = 5;

	[Export]
	public float yBulletSpeed = 5;

	[Export]
	public float telegraphTime = 2;

	private float ttTimeElapsed = 0;

	private bool telegraphing = true;
	public override void _Ready()
	{
		
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if(telegraphing){
			ttTimeElapsed += delta;
			telegraphing = (ttTimeElapsed < telegraphTime);
			return;
		}else{
			Translate(new Vector2(xBulletSpeed, yBulletSpeed));
		}
	}

	
}
