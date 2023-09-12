using Godot;
using System;
using System.Collections.Generic;
using static GameplayUtilities;

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

	private InteractZone zoneOnDeck;
	private List<InteractZone> areasWithin;

	Vector2 inputDir;

	ExploreMode myExploreMode;

	GameplayMode nextMode;

	public override void _Ready(){
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
		areasWithin = new List<InteractZone>();
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

	public void HandleInput(PlayerInput input){
		if(input == PlayerInput.Select){
			if(zoneOnDeck != null) myExploreMode.SetModeOnDeck(zoneOnDeck.Activate());
		}
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
		if(area is CutsceneInteractZone){
			areasWithin.Add(area as InteractZone);
			zoneOnDeck = area as CutsceneInteractZone;
		}
	}

	public void OnBodyAreaEntered(Area3D area){

	}

	public void OnInteractionAreaExited(Area3D area){
		if(area is InteractZone){
			if(areasWithin.Contains(area as InteractZone)){
				areasWithin.Remove(area as InteractZone);
			}
			if(zoneOnDeck == area){
				if(areasWithin.Count > 0){
					zoneOnDeck = areasWithin[0];
				}else{
					zoneOnDeck = null;
				}
			}
		}
	}

	public void SetExploreMode(ExploreMode mode){
		myExploreMode = mode;
	}
}
