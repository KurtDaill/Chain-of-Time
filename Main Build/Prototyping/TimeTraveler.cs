using Godot;
using System;
using System.Threading.Tasks;

public partial class TimeTraveler : Node
{
	/*
		Procedure for traveling through time:
		1. Player Interacts with Object in Game World
		2. Everything Starts Slowing Down (Except Cato) - Done
		3. Everything Ends up Stopped (Except Cato) - Done
		4. The Engine Takes a Picture of the stopped scene - Done
		5. The Engine Displys that Picture to camera with Cato still animating over the spot that he was in the 3-D Scene - Done
		6. We Load in the Past Scene Behind the Picture, as we do some kind of visual effect to the picture
		7. The Picture fades out in some way, Cato is in the correct spot in the current Scene, player regains control and goes about their merry way - Half Done (Cato's Position is still good)
	*/
	/*
		Problems/To Do's:
		1. How do we put Cato on top of a rendered scene like that correctly?
		2. How does that rendered scene fit correctly with the scene it was a screenshot of.
	*/
	bool inSlowDown = false;
	[Export]
	double slowDownTimeInSeconds = 1;
	double timer = 0;
	[Export]
	Sprite3D CatoSprite;
	[Export]
	MeshInstance3D renderedScenePlane;
	Texture2D screenCaptureTexture;
	[Export]
	Node3D present;
	[Export]
	Node3D past;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		renderedScenePlane.Visible=false;
		present.Visible = true;
		past.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public async override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("debug_5")){
			inSlowDown = true;
			timer = slowDownTimeInSeconds;
		}
		if(inSlowDown){
			timer -= delta;
			if(timer < 0) timer = 0;
			foreach(Node node in GetTree().GetNodesInGroup("AnimationPlayer")){
				if(node.GetGroups().Contains("Cato")) continue; //We don't slow down Cato.
				if(node is AnimationPlayer player) 
					player.SpeedScale = (float)(timer/slowDownTimeInSeconds);
			}
			//GetViewport().GetCamera3D().Fov = (float)(75 * (timer/slowDownTimeInSeconds) + 35 * (1-timer/slowDownTimeInSeconds)); 
			//Camera3D camera = GetViewport().GetCamera3D();
			//camera.Position = new Vector3(GetViewport().GetCamera3D().Position.X, GetViewport().GetCamera3D().Position.Y, (float)(8.594 * (timer/slowDownTimeInSeconds) + 6.2 * (1-timer/slowDownTimeInSeconds)));
			if(timer == 0){
				inSlowDown = false;
				await CaptureCurrentSceneAndSetFacade();
				present.Visible = false;
				past.Visible = true;
				renderedScenePlane.GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeIn");
				await ToSignal(renderedScenePlane.GetNode<AnimationPlayer>("AnimationPlayer"), AnimationPlayer.SignalName.AnimationFinished);
			}
		}
	}

	private async Task CaptureCurrentSceneAndSetFacade(){
		//Capture the Screen...Somehow
		CatoSprite.Visible = false;
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		//GetViewPort().GetTexture() would keep live udpating, making the image like a screen almost...
		//screenCaptureTexture = GetViewport().GetTexture();
		screenCaptureTexture = ImageTexture.CreateFromImage(GetViewport().GetTexture().GetImage())	;
		((QuadMesh)renderedScenePlane.Mesh).Material.Set("albedo_texture", screenCaptureTexture);
		renderedScenePlane.Visible = true;

		//Get the Position Data for the Plane
		Vector3 TopLeftCorner = GetViewport().GetCamera3D().ProjectPosition(Vector2.Zero, 1);
		Vector3 BottomRightCorner = GetViewport().GetCamera3D().ProjectPosition(GetViewport().GetVisibleRect().Size, 1);
		((QuadMesh)renderedScenePlane.Mesh).Size = new Vector2(Mathf.Abs(BottomRightCorner.X - TopLeftCorner.X), Mathf.Abs(TopLeftCorner.Y - BottomRightCorner.Y));
		CatoSprite.Set("no_depth_test", true);
		CatoSprite.Visible = true;
		//((QuadMesh)renderedScenePlane.Mesh).Material.Set("albedo_color", Godot.Color.FromHtml("#ffffffff"));
	}
}
