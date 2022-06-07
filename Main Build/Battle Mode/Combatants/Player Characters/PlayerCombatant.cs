using Godot;
using System;

public abstract class PlayerCombatant : Combatant
{
    public int strength = 10;
    public bool rightFace = true;
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
        if(hSpeed > 0){
            facing = 1;
        }else if (hSpeed < 0){
            facing = -1;
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

