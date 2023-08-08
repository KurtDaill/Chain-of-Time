using Godot;
using System;

public partial class TimeTraveler : Node
{
	/*
		Procedure for traveling through time:
		1. Player Interacts with Object in Game World
		2. Everything Starts Slowing Down (Except Cato)
		3. Everything Ends up Stopped (Except Cato)
		4. The Engine Takes a Picture of the stopped scene
		5. The Engine Displys that Picture to camera with Cato still animating over the spot that he was in the 3-D Scene
		6. We Load in the Past Scene Behind the Picture, as we do some kind of visual effect to the picture
		7. The Picture fades out in some way, Cato is in the correct spot in the current Scene, player regains control and goes about their merry way
	*/
	/*
		Problems/To Do's:
		1. How do we put Cato on top of a rendered scene like that correctly?
		2. How does that rendered scene fit correctly with the scene it was a screenshot of.
	*/
	bool inSlowDown = false;
	[Export]
	MeshInstance3D renderedScenePlane;
	Texture2D screenCaptureTexture;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		renderedScenePlane.Visible=false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public async override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("debug_5")){
			//Capture the Screen...Somehow
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			//GetViewPort().GetTexture() would keep live udpating, making the image like a screen almost...
			screenCaptureTexture = GetViewport().GetTexture();
			//screenCaptureTexture = ImageTexture.CreateFromImage(GetViewport().GetTexture().GetImage())	;
			((QuadMesh)renderedScenePlane.Mesh).Material.Set("albedo_texture", screenCaptureTexture);
			renderedScenePlane.Visible = true;

			//Get the Position Data for the Plane
			Vector3 TopLeftCorner = GetViewport().GetCamera3D().ProjectPosition(Vector2.Zero, 1);
			Vector3 BottomRightCorner = GetViewport().GetCamera3D().ProjectPosition(GetViewport().GetVisibleRect().Size, 1);
			((QuadMesh)renderedScenePlane.Mesh).Size = new Vector2(Mathf.Abs(BottomRightCorner.X - TopLeftCorner.X), Mathf.Abs(TopLeftCorner.Y - BottomRightCorner.Y));
			//((QuadMesh)renderedScenePlane.Mesh).Material.Set("albedo_color", Godot.Color.FromHtml("#ffffffff"));
		}
	}
}
