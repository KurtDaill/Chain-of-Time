using Godot;
using System;

public partial class BattleNotification : Node3D
{
	public async override void _Ready(){
		if(!this.GetNode<AnimationPlayer>("AnimationPlayer").HasAnimation("Notify")){
			GetTree().Quit();
			throw new SystemException();
		}
		this.GetNode<AnimationPlayer>("AnimationPlayer").Play("Notify");
		await ToSignal(this.GetNode<AnimationPlayer>("AnimationPlayer"), AnimationPlayer.SignalName.AnimationFinished);
		this.QueueFree();
	}
}
