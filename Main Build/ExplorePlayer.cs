using Godot;
using System;

public partial class ExplorePlayer : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private AnimationPlayer animPlay;

	private Vector3 direction;

	public override void _Ready(){
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		direction = (Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.x = direction.x * Speed;
			velocity.z = direction.z * Speed;
		}
		else
		{
			velocity.x = Mathf.MoveToward(Velocity.x, 0, Speed);
			velocity.z = Mathf.MoveToward(Velocity.z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if(direction.x != 0){
			if(direction.x > 0){
				animPlay.Play("Walk Right");
			}else{
				animPlay.Play("Walk Left");
			}
		}else if(direction.z != 0){
			if(direction.z > 0){
				animPlay.Play("Walk Down");
			}else{
				animPlay.Play("Walk Up");
			}
		}else{
			animPlay.Play("Idle");
		}
    }
}
