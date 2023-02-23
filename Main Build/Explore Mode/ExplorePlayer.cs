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

	private TimeFragment timeFrag;

	public override void _Ready(){
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		if(!inControl) return;
		Vector3 velocity = Velocity;

		if(timeFrag != null && Input.IsActionJustPressed("com_atk")){
			timeFrag.TimeTravel(this);
		}

		// Add the gravity.
		if (!IsOnFloor())
			velocity.y -= gravity * (float)delta;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		direction = (Transform.basis * new Vector3(inputDir.x, 0, inputDir.y)).Normalized();
		direction *= Speed;
		velocity = new Vector3(direction.x, velocity.y, direction.z);
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

	public void SetPlayerControl(bool set){
		inControl = set;
	}

	public void SetActive(bool active){
		SetPlayerControl(active);
		this.Visible = active;
	}

	public void OnInteractionAreaEntered(Area3D area){
		if(area.GetGroups().Contains("Time Fragment")){
			timeFrag = (TimeFragment) area;
			if(((TimeFragment)area).ArmTimeFragment())timeFrag = (TimeFragment)area;
		}
		if(area.GetGroups().Contains("Encounter")){
			Encounter en = (Encounter) area;
			en.StartEncounter(this);
		}
	}

	public void OnInteractionAreaExited(Area3D area){
		if(area.GetGroups().Contains("Time Fragment")){
			((TimeFragment)area).DisarmTimeFragment();
		}
	}
}
