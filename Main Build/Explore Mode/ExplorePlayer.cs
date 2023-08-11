using Godot;
using System;

public partial class ExplorePlayer : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;

	[Export]
	public Camera3D exploreCamera;

	[Export]
	private float waypointSpacing = 2;

	[Export(PropertyHint.File)]
	private string waypointRes;
	[Export]
	private ChronoEnviro timeEnvironment;

	public bool inControl = true;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private AnimationPlayer animPlay;

	private Vector3 direction;

	private Area3D timeFrag;

	private ExploreNPC npcInRange;

	private InteractionPoint pointInRange;

	private Waypoint[] waypoints = new Waypoint[3];
	[Export]
	TimeTraveler timeObject;

	private int promenade;

	public override void _Ready(){
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
		promenade = 0;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(!inControl) return;
		Vector3 velocity = Velocity;

		if(Input.IsActionJustPressed("ui_proceed")){
			if(timeFrag != null){
				timeObject.Activate();
				return;
				if(npcInRange != null){
					npcInRange.DisarmCutscene();
					npcInRange = null;
				}
				timeFrag = null;
			}else if(npcInRange != null){
				npcInRange.PlayCutscene();
			}else if (pointInRange != null){
				pointInRange.PlayCutscene();
			}
		}

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		direction *= Speed;
		velocity = new Vector3(direction.X, velocity.Y, direction.Z);
		Velocity = velocity;
		MoveAndSlide();
		//if(!timeEnvironment.IsInPast()) ManageFollowerWaypoints();
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
		if(area.GetGroups().Contains("Time Fragment")){
			timeFrag = area;
		}
		if(area.GetGroups().Contains("Encounter")){
			Encounter en = (Encounter) area;
			//en.StartEncounter(this);
		}
		if(area.GetGroups().Contains("NPC")){
			ExploreNPC npc = (ExploreNPC) area.GetParent();
			if(npc.ArmCutscene()){
				npcInRange = npc;
			}
		}
		if(area.GetGroups().Contains("Interaction Points")){
			InteractionPoint point = (InteractionPoint) area.GetParent();
			if(point.ArmCutscene()){
				pointInRange = point;
			}
		}
		
	}

	public void OnBodyAreaEntered(Area3D area){
		if(area.GetGroups().Contains("Promenade")){
			promenade = ((Promenade)area).GetPromenadeIndex();
		}
		if(area.GetGroups().Contains("Temp")){
			//Temp temporarySolution = (Temp) area;
			//temporarySolution.OnAreaEntered();
		}
		if(area.GetGroups().Contains("Transition")){
			LevelTransition trans = (LevelTransition) area;
			trans.Transition();
		}
	}

	public void OnInteractionAreaExited(Area3D area){
		if(area.GetGroups().Contains("Time Fragment")){
			//((TimeFragment)area).DisarmTimeFragment();
		}
		if(area.GetGroups().Contains("NPC")){
			ExploreNPC npc = (ExploreNPC) area.GetParent();
			npc.DisarmCutscene();
		}
		if(area.GetGroups().Contains("Interaction Points")){
			InteractionPoint point = (InteractionPoint) area.GetParent();
			point.DisarmCutscene();
		}
	}

	public void ManageFollowerWaypoints(){
		if(waypoints[0] == null || Mathf.Abs((waypoints[0].GlobalPosition - this.GlobalPosition).Length()) > waypointSpacing){
			if(waypoints[2] != null) waypoints[2].QueueFree();
			waypoints[2] = waypoints[1];
			waypoints[1] = waypoints[0];
			waypoints[0] = GD.Load<PackedScene>(waypointRes).Instantiate<Waypoint>();
			waypoints[0].GlobalPosition = this.GlobalPosition;
			waypoints[0].SetPromenade(promenade);
			this.GetParent().AddChild(waypoints[0]);
		}
	}

	public Waypoint GetWaypoint(int waypoint){
		return waypoints[waypoint];
	}

	public int GetCurrentPromenade(){
		return promenade;
	}

	public void SetPromenade(int i ){ //TEMP CODE
		promenade = i;
	}
}
