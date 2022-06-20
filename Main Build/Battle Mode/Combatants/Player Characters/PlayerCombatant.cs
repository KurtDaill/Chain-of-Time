using Godot;
using System;
using System.Collections.Generic;

public abstract class PlayerCombatant : Combatant
{
    public int strength = 10;
    public bool rightFace = true;
    protected List<Hitbox> hitboxes = new List<Hitbox>();
    [Export]
    public string character = "Cato";
    [Export]
	public float jumpForce = 200F;
	[Export]
	public float runSpeed = 150F;
	[Export]
	public float dashBoost = 200F;
	[Export]
	public float diveSpeed = 3F;
	[Export]
	public float slideDrag = 2F;
	[Export]
	public float footDrag = 9F;
	[Export]
	public float dashDrag = 5F;

    
    public bool crouching = false;
    public bool dashing = false;

    public override void _Ready()
    {
        state = new CombatantStateStandby();
        animTree = (AnimationTree) GetNode("./AnimationTree");
        animSM = (AnimationNodeStateMachinePlayback) animTree.Get("parameters/playback");
        animPlayer = (AnimationPlayer) GetNode(animationPlayer);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(hSpeed > 0){
            facing = 1;
        }else if (hSpeed < 0){
            facing = -1;
        }
        updateAnimationTree();
    }

    
    

    
    public abstract void spawnHitbox(String requestedHitbox);

    public virtual void clearHitboxes(){
        foreach(Area box in hitBoxes){
            box.QueueFree();
        }
    }

    public virtual void Move()
    {
        if(state == null){
            throw new ArgumentNullException();
        }
        
        newState = state.Process(this);
        if(newState != null) SetState(newState);
        newState = null;
    }

    //Ran when the character in question is able to move and attack. Returns true once they're finished.
    //Targets & damageRecord are used to track who is damaged this attack and how much
    //They're given from the command running move and attack, and their ultimate values are stores in that command
    public abstract bool MoveAndAttack(EnemyCombatant[] targets, int[] damageRecord);
}

