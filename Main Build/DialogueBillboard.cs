using Godot;
using System;

public partial class DialogueBillboard : MeshInstance3D
{
	[Export]
	public SubViewport targetViewPort;
	// Called when the node enters the scene tree for the first time.
	public async override void _Ready()
	{
		await ToSignal(RenderingServer.Singleton, "frame_post_draw");
		StandardMaterial3D mat = (StandardMaterial3D)GD.Load<StandardMaterial3D>("res://3DTextboxTexture.tres").Duplicate();
		mat.AlbedoTexture = targetViewPort.GetTexture();
		this.MaterialOverride = mat;
		this.Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
