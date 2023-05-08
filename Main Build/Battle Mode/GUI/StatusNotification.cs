using Godot;
using System;

public partial class StatusNotification : Node3D
{
	AnimationPlayer animPlay;
	public override void _Ready(){
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
	}

    public void PlayNotification(Node3D spawnPoint){
		this.GlobalPosition = spawnPoint.GlobalPosition;
		animPlay.Play("Notify");
	}
}
