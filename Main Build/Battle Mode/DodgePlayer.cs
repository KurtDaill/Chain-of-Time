using Godot;
using System;

public class DodgePlayer : PlayerCombatant
{
    DodgePlayerState state = new DodgePlayerStateGround();
    DodgePlayerState newState = null;
    AnimatedSprite sprite;

    Area2D hitbox;
    public bool rightFace = true;
    [Export]
    public string character = "Cato";
    [Export]
	public float jumpForce = 10F;
	[Export]
	public float runSpeed = 2F;
	[Export]
	public float dashBoost = 10F;
	[Export]
	public float diveSpeed = 3F;
	[Export]
	public float gravity = 3F;
	[Export]
	public float slideDrag = 0.1F;
	[Export]
	public float airDrag = 0.05F;
	[Export]
	public float footDrag = 0.3F;
	[Export]
	public float dashDrag = 0.3F;

    public float hSpeed = 0;
    public float vSpeed = 0;
    public override void _Ready()
    {
        Godot.Collections.Array children = GetChildren();
        for(int i = 0; i < children.Count; i++){
            if(children[i] is AnimatedSprite){
                sprite = (AnimatedSprite) children[i];
            }
            if(children[i] is Area2D){
                hitbox = (Area2D) children[i];
            }
        }
        sprite.Connect("animation_finished", this, nameof(HandleAnimationTransition));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        //Disabled while testing Command-Pattern solution for battles
        /*
        //ProcessPlayerState()    Runs the current state script 
        newState = state.Process(this);
        if(newState != null){
            DodgePlayerState temp = state;
            state = newState;
            state.Enter(this, temp);
        }
        newState = null;
        */
    }

    public override void DefensiveMovement()
    {
        newState = state.Process(this);
        if(newState != null){
            DodgePlayerState temp = state;
            state = newState;
            state.Enter(this, temp);
        }
        newState = null;
    }

    public void setSprite(String newSprite, int scaling = 1){
        sprite.Animation = newSprite;
        if(rightFace){
            sprite.Scale = new Vector2(1, 1);
        }else{
            sprite.Scale = new Vector2(-1, 1);
        }
        //sprite.Scale = new Vector2(scaling, 1);
    }

    public AnimatedSprite GetAnimatedSprite(){
        return sprite;
    }

    //Called whenever an animation ends, and calls a function in the state that handles any new animations that have to play
    //For Example: The Airborne state will transition from the "Up" to "Down" animation once the player begins falling
    private void HandleAnimationTransition(){
        state.HandleAnimationTransition(this);
    }

    //Should return whether or not the player is on the ground
    public bool amIFlying(){ //TODO implement this function
        KinematicCollision2D kc = GetSlideCollision(0);
        if(kc != null){
			if(kc.Collider.GetMeta("type").Equals("ground")){
				return false;
			}
		}
        return true;
    }

    public void setNewHitbox(string newBox){
        CollisionShape2D curBox;
        Godot.Collections.Array boxes = hitbox.GetChildren();
        for(int i = 0; i < boxes.Count; i++){
            curBox = (CollisionShape2D) boxes[i];
            curBox.Disabled = true;
            if(curBox.Name == newBox){
                curBox.Disabled = false;
                return;
            } 
        }
    }
}
