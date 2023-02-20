using Godot;
using System;

public partial class Actor : Node
{
	[Export]
	string name = "Actor";

	MeshInstance3D balloon;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override void _Ready(){
		balloon = this.GetNode<MeshInstance3D>("Dialogue Balloon");
	}

	public string GetActorName(){
		return name;
	}

	public void HideBalloon(){
		balloon.Visible = false;
	}

	public void ShowBalloon(){
		balloon.Visible = true;
	}

	public void SetVisiblity(bool vis){
		this.GetNode<Sprite3D>("Sprite").Visible = vis;
	}
}
