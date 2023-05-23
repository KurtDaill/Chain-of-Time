using Godot;
using System;

public partial class FollowerNPCcs : CharacterBody3D
{
	[Export]
	private int marchingOrder = 0;

	[Export]
	private float personalSpaceRadius = 0.75F;

	[Export]
	private float waypointAccuracyTolerance = 0.3F;

	private ExplorePlayer player;

	[Export]
	public float Speed = 2.0f;

	private AnimationPlayer animPlay;

	private int currentPromenade = 0;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private bool followerEnabled = true;

	public override void _Ready(){
		player = this.GetParent<SceneConfig>().GetExplorePlayer();
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		// Handle Jump.
		//if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			//velocity.y = JumpVelocity;

			
		if (!IsOnFloor() && followerEnabled)
			velocity.Y -= gravity * (float)delta;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		//Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Waypoint waypoint = player.GetWaypoint(marchingOrder);
		if(player.GetWaypoint(marchingOrder) != null &&
		followerEnabled &&
		MathF.Abs((waypoint.GlobalPosition - this.GlobalPosition).Length()) > waypointAccuracyTolerance &&
		Mathf.Abs((this.GlobalPosition - player.GlobalPosition).Length()) > personalSpaceRadius         &&
		waypoint.promenadeIndex == this.currentPromenade ){	
			Vector3 direction = (waypoint.GlobalPosition - this.GlobalPosition).Normalized();
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;	
		}else{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}
		Velocity = velocity;
		MoveAndSlide();
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if(Velocity != Vector3.Zero){
			if(Mathf.Abs(Velocity.X) > Mathf.Abs(Velocity.Z)){
				if(Velocity.X > 0){
					animPlay.Play("Walk Right");
				}else{
					animPlay.Play("Walk Left");
				}
			}else{
				if(Velocity.Z > 0){
					animPlay.Play("Walk Down");
				}else{
					animPlay.Play("Walk Up");
				}
			}
		}else{
			animPlay.Play("Idle");
		}
    }

	public void Disable(){
		this.Visible = false;
		this.GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;
		followerEnabled = false;
	}

	public void Enable(){
		this.Visible = true;
		this.GetNode<CollisionShape3D>("CollisionShape3D").Disabled = false;
		followerEnabled = true;
	}
}
