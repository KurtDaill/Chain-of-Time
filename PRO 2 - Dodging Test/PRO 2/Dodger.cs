using Godot;
using System;

public class Dodger : KinematicBody2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	private enum State  
	{
		Ground, 
		Jump,
		Dive,
		Crouch,
		Slide
	};

	private enum AnimState
	{
		Ground,
		JumpUp,
		FallDown,
		Splat,

		Crouching,
		Crouch,
		Uncrouching,
		Dashing
	}
	[Export]
	public float jumpForce = 10F;
	[Export]
	public float runSpeed = 2F;
	[Export]
	public float dashBoost = 10F;
	[Export]
	public float diveSpeed = 3F;
	[Export]
	public float Gravity = -0.5F;
	[Export]
	public float slideDrag = 0.1F;
	[Export]
	public float airDrag = 0.05F;
	[Export]
	public float footDrag = 0.3F;
	[Export]
	public float dashDrag = 0.3F;

	private float vSpeed = 0F;
	private float hSpeed = 0F;

	private bool dashing = false;

	//private animation? curAnim = Idle

	private State DodgerState =  State.Jump;
	private AnimState AnimationState = AnimState.Ground;

	private AnimatedSprite sprite;

	private KinematicCollision2D kc = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = (AnimatedSprite) GetChild(0);
	}


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		switch(DodgerState){
			case State.Ground :
				if(Input.IsActionPressed("ui_down")){
					DodgerState = State.Slide;
				}
				if(dashing){ //if we're on the ground and dashing: no player input and no other forces.
					if(dashing) hSpeed = Math.Sign(hSpeed) * (Math.Abs(hSpeed) - dashDrag);
					break;
				}
				if(Input.IsActionPressed("ui_caps")){ //Dashing
					if(Input.IsActionPressed("ui_left")){
						hSpeed -= dashBoost;  
					}else{
						hSpeed += dashBoost;
					}
					dashing = true;
				}
				else{ //Running movement, player should slide if there's no input held
					if(Input.IsActionPressed("ui_right")){
						hSpeed = runSpeed;
					}else if(Input.IsActionPressed("ui_left")){
						hSpeed = -runSpeed;
					}else{
						hSpeed = Math.Sign(hSpeed) * Math.Max((Math.Abs(hSpeed) - footDrag), 0);
					}
				}
				if(Input.IsActionPressed("ui_up")){ //Jumping
					vSpeed = -jumpForce;
					DodgerState = State.Jump;
				}
				break;

			case State.Jump :
				//Player Should Fall while also losing some horizontal speed due to drag
				vSpeed -= Gravity;

				if(dashing){ //Dashing
					hSpeed = Math.Sign(hSpeed) * (Math.Abs(hSpeed) - dashDrag);
				}else{ //Not Dashing (Allows for the player to start dashing, and has different Drag)
					hSpeed = Math.Sign(hSpeed) * Math.Max(Math.Abs(hSpeed) - airDrag, 0);

					if(Input.IsActionPressed("ui_caps")){//Allows for a midair dash
						if(Input.IsActionPressed("ui_left")){
							hSpeed -= dashBoost;  
						}else{
							hSpeed += dashBoost;
						}
						dashing = true;
					}
				}
				if(Input.IsActionPressed("ui_down")){//Allows for a midair dive
					DodgerState = State.Dive;
					vSpeed -= (diveSpeed * 8);
					hSpeed = Math.Sign(hSpeed) * (Math.Abs(hSpeed) - (diveSpeed * 2 ));
				}
				break;

			case State.Crouch :
				vSpeed = 0;
				hSpeed = 0;
				sprite.Animation = "crouch";
					if(!Input.IsActionPressed("ui_down")){
					DodgerState = State.Ground;
					sprite.Animation = "default";
				}
				/*
					VSpeed = 0;
					HSpeed = 0;
					animation = Crouch
					Set Hitbox to small?
					if player isn't holding down ->
						goto Ground
					
				*/
				break;

			case State.Slide :
				sprite.Animation = "crouch";
				if(dashing) hSpeed = Math.Sign(hSpeed) * (Math.Abs(hSpeed) - dashDrag);
				else hSpeed = Math.Sign(hSpeed) * Math.Max((Math.Abs(hSpeed) - slideDrag), 0);
				if(hSpeed == 0){
					DodgerState = State.Crouch;
				}
				if(Input.IsActionPressed("ui_up")){ //Jumping
					vSpeed = -jumpForce;
					DodgerState = State.Jump;
					sprite.Animation = "default";
				}
				if(!Input.IsActionPressed("ui_down")){
					DodgerState = State.Ground;
					sprite.Animation = "default";
				}
				break;

			case State.Dive :
				vSpeed -= diveSpeed;
				if(dashing)hSpeed = Math.Sign(hSpeed) * (Math.Abs(hSpeed) - dashDrag);
				else hSpeed = Math.Sign(hSpeed) * (Math.Abs(hSpeed) - diveSpeed);
				/*
				VSpeed -= DiveSpeed;
				HSpeed -= AirDrag;
				if(onGround)
					goto Ground
				*/
				break;
		}
		
		MoveAndSlide(new Vector2(hSpeed, vSpeed));

		//Gets the info from the most recent collision
		kc = GetSlideCollision(0);
		if(kc != null){
			if(kc.Collider.GetMeta("type") == null){
				dashing = false;
				Translate(new Vector2(-Math.Sign(hSpeed), 0));
			}else if(kc.Collider.GetMeta("type").Equals("ground")){
				if(DodgerState != State.Crouch && DodgerState != State.Slide) DodgerState = State.Ground;
			}
		}
		if(Math.Abs(hSpeed) - runSpeed < 0){
			dashing = false;
		}

		switch(AnimationState){
			case AnimState.Ground :
				break;
			case AnimState.JumpUp :
				break;
			case AnimState.FallDown :
				break;
			case AnimState.Splat :
				break;
			case AnimState.Crouching :
				break;
			case AnimState.Crouch :
				break;
			case AnimState.Uncrouching :
				break;
			case AnimState.Dashing :
				break;
		}
	}
	private void onHitboxAreaEntered(Area2D area){
		Godot.Collections.Array groups = area.GetGroups();
		foreach(String group in groups){
			if(group == "bullet"){
				GD.Print("Oww!");
			}   
		}
	}
}




