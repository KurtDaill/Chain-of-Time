using Godot;
using System;

public abstract class PlayerCombatant : Combatant
{
    protected PlayerCombatantState state = new PlayerCombatantStateExit();
    protected PlayerCombatantState newState = null;
    protected AnimatedSprite sprite;

    protected String queuedSprite = null;

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
	public float airDrag = 0.21F;
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

    public void setSprite(String newSprite, int scaling = 1){
        sprite.Animation = newSprite;
        if(rightFace){
            sprite.Scale = new Vector2(1, 1);
        }else{
            sprite.Scale = new Vector2(-1, 1);
        }
        //sprite.Scale = new Vector2(scaling, 1);
    }

    public void queueSprite(string queueMe){
        queuedSprite = queueMe;
    }

    public AnimatedSprite GetAnimatedSprite(){
        return sprite;
    }

    //Called whenever an animation ends, and calls a function in the state that handles any new animations that have to play
    //For Example: The Airborne state will transition from the "Up" to "Down" animation once the player begins falling
    private void HandleAnimationTransition(){
        if(state != null) 
        state.HandleAnimationTransition(this);
        else if(queuedSprite != null){
            setSprite(queuedSprite);
            queuedSprite = null;
        }
        else 
            return;
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

    public void SetAnim(string anim){
        sprite.Animation = anim;
    }

    public void SetState(PlayerCombatantState newState){
        state.Exit();
        PlayerCombatantState temp = state;
        state = newState;
        state.Enter(this, temp);
    }
}

