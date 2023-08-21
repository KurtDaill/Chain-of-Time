using Godot;
using System;

public partial class ExplorePlayer : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;

	[Export]
	public Camera3D exploreCamera;
	public bool inControl = true;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private AnimationPlayer animPlay;

	private Vector3 direction;
	[Export]
	TimeTraveler timeObject;

	Vector2 inputDir;

	public override void _Ready(){
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		if(!inControl) return;
		Vector3 velocity = Velocity;

		if(Input.IsActionJustPressed("ui_proceed")){

		}

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		
		direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		direction *= Speed;
		velocity = new Vector3(direction.X, velocity.Y, direction.Z);
		Velocity = velocity;
		MoveAndSlide();
		//if(!timeEnvironment.IsInPast()) ManageFollowerWaypoints();
	}

	public void HandleInput(){
		inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if(direction.X != 0){
			if(direction.X > 0){
				animPlay.Play("Walk Right");
			}else{
				animPlay.Play("Walk Left");
			}
		}else if(direction.Z != 0){
			if(direction.Z > 0){
				animPlay.Play("Walk Down");
			}else{
				animPlay.Play("Walk Up");
			}
		}else{
			animPlay.Play("Idle");
		}
    }

	public void SetPlayerControl(bool set){
		inControl = set;
	}

	public void SetActive(bool active){
		SetPlayerControl(active);
		this.Visible = active;
	}

	public void OnInteractionAreaEntered(Area3D area){
		
	}

	public void OnBodyAreaEntered(Area3D area){

	}

	public void OnInteractionAreaExited(Area3D area){
	}
}
