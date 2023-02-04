using Godot;
using System;

public partial class Temp : Node
{
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		await ToSignal(RenderingServer.Singleton, "frame_post_draw");
		StandardMaterial3D mat = GD.Load<StandardMaterial3D>("res://3DTextboxTexture.tres");
		mat.AlbedoTexture = GetNode<SubViewport>("/root/Node3D/Character Ballon/SubViewportContainer/Ballon VP").GetTexture();
		this.GetNode<MeshInstance3D>("Label Plane").MaterialOverride = mat;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("ui_accept")){
			MeshInstance3D lp = this.GetNode<MeshInstance3D>("Label Plane");
			StandardMaterial3D mat = new StandardMaterial3D();
			ViewportTexture vpText = new ViewportTexture();

			vpText.ViewportPath = ("Character Ballon/SubViewportContainer/Ballon VP");
			var check = GetNode("/root/Node3D/Character Ballon/SubViewportContainer/Ballon VP");
			//await ToSignal(GetNode<SubViewport>("/root/Node3D/Character Ballon/SubViewportContainer/Ballon VP"), "FramePostDraw");
			mat.AlbedoTexture = GetNode<SubViewport>("/root/Node3D/Character Ballon/SubViewportContainer/Ballon VP").GetTexture();
			
			lp.MaterialOverride = mat;
			
		}
	}
}
