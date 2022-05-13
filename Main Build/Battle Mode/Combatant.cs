using Godot;
using System;

public abstract class Combatant : KinematicBody2D {
    private int hitPoints;
    private int maxHP;
    protected CombatantState state = new CombatantStateStandby();
    protected CombatantState newState = null;
    protected AnimatedSprite sprite;
    protected String queuedAnimation = null;
    protected int armor = 0;

    //Equals 1 while facing right, -1 while facing left, used in calculations dependent on character's facing
    public int facing = 1;  

    [Export]
	public float gravity = 9F;

    [Export]
    public float knockbackDrag = 3F;

    [Export]
    public float knockbackGravity = 3F;

	[Export]
	public float airDrag = 0.21F;

    //On a scale of 0-1 how much do we reduce knockback inflicted on this character. 
    [Export]
    public float knockbackResist = 0F;

    protected Area2D hitbox;

    public float hSpeed = 0;
    public float vSpeed = 0;

    public int getHP(){
        return hitPoints;
    }

    public int getMaxHP(){
        return maxHP;
    }

    public void modifyMaxHP(int mod){
        if(mod > 0 && hitPoints == maxHP){
            maxHP += mod;
            hitPoints += mod;
        }else{
            maxHP += mod;
            if(hitPoints > maxHP){
                hitPoints = maxHP;
            }
        }
    }

    //Returns the ammount of damage the combatant takes
    public virtual int TakeDamage(int incomingDamage, Vector2 knockback){
        int damage = Math.Max(0, incomingDamage - armor);
        hitPoints -= damage;
        //TODO Damage Numbers and other effects
        knockback *= (1-knockbackResist);
        hSpeed += knockback.x;
        vSpeed += knockback.y;
        return damage;
    }

    public void recoverHP(int heal){
        if(heal <= 0){
            throw new NegativeHealingException();
        }
        hitPoints += heal;
        if(hitPoints >= maxHP){
            hitPoints = maxHP;
        }
    }

    //Returns true when the player is in the air
    public bool AmIFlying(){
        if(GetSlideCount() == 0) return true;

        KinematicCollision2D kc = GetSlideCollision(0);
        if(kc.Collider != null) return false;
        return true;
    }
    
    //TODO retrofit states to use set sprite
    //TODO refactor code to properly handle
    public void setSprite(String newSprite, int scaling = 1){
        sprite.Animation = newSprite;
        sprite.Scale = new Vector2(scaling, 1);
        //sprite.Scale = new Vector2(scaling, 1);
        facing = scaling;
    }

    public void queueSprite(string queueMe){
        queuedAnimation = queueMe;
    }

    public AnimatedSprite GetAnimatedSprite(){
        return sprite;
    }

    //Called whenever an animation ends, and calls a function in the state that handles any new animations that have to play
    //For Example: The Airborne state will transition from the "Up" to "Down" animation once the player begins falling
    protected void HandleAnimationTransition(){
        if(state != null) 
        state.HandleAnimationTransition(this);
        else if(queuedAnimation != null){
            setSprite(queuedAnimation);
            queuedAnimation = null;
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

    public void SetState(CombatantState newState){
        state.Exit();
        CombatantState temp = state;
        state = newState;
        state.Enter(this, temp);
    }
}

[Serializable]
public class NegativeHealingException : Exception
{
    public NegativeHealingException() : base() { }
    public NegativeHealingException(string message) : base(message) { }
    public NegativeHealingException(string message, Exception inner) : base(message, inner) { }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client.
    protected NegativeHealingException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}