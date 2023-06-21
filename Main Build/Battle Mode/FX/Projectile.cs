using Godot;
using System;

public partial class Projectile : Node3D
{
	[Export]
	double topSpeed;
	[Export]
	double acceleration = -1;
	[Export]
	double contactTolerance = 0.5;
	[Export]
	ProjectilePathType pathType = ProjectilePathType.Straight;
	[Signal]
	public delegate void TargetHitEventHandler();

	Node3D start, target;
	double speed;
	AnimationPlayer animPlay;
	bool expiring = false;

	enum ProjectilePathType{
		Straight,
		Arc
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(pathType == ProjectilePathType.Arc) throw new NotImplementedException();
	} 
	
	public virtual void Setup(Battle battle, Combatant setupStart, int startBodyIndex, Combatant setupTarget, int targetBodyIndex){
		battle.AddChild(this);
		start = setupStart.GetBodyRegion(startBodyIndex);
		target = setupTarget.GetBodyRegion(targetBodyIndex);
		this.GlobalPosition = start.GlobalPosition;
		this.animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
		animPlay.Play("Fire");
		animPlay.AnimationFinished += OnAnimationFinish;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(expiring) return;
		if(acceleration == -1){
			speed = topSpeed;
		}else{
			if(speed < topSpeed){
				speed += acceleration * delta;
				speed = Math.Min(speed, topSpeed);
			}	
		}
		Vector3 vectorToTaget = (target.GlobalPosition - this.GlobalPosition).Normalized();
		this.Position += vectorToTaget * ((float)speed * (float)delta);
		if((target.GlobalPosition - this.GlobalPosition).Length() < contactTolerance){
			this.animPlay.Play("Hit");
			EmitSignal(Projectile.SignalName.TargetHit);
			expiring = true;
		}
	}

	public void OnAnimationFinish(StringName lastAnim){
		switch(lastAnim){
			case "Fire" :
				animPlay.Play("Loop");
				break;
			case "Hit" :
				this.Expire();
				break;
		}
	}
	
	private void Expire(){
		this.QueueFree();
	}
}
