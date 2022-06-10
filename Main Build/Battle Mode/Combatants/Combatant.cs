using Godot;
using System;

public abstract class Combatant : KinematicBody {
    private int hitPoints;
    private int maxHP;
    protected CombatantState state = new CombatantStateStandby();
    protected CombatantState newState;
    public AnimationTree animTree;
    public AnimationNodeStateMachinePlayback animSM;
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
    public virtual int TakeDamage(int incomingDamage, Vector3 knockback){
        int damage = Math.Max(0, incomingDamage - armor);
        hitPoints -= damage;
        SetState(new CombatantStatePain(this, (knockback *(1-knockbackResist)), incomingDamage));
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
    public virtual void updateAnimationTree(){
        animTree.Set("parameters/conditions/isFalling", vSpeed < 10);
        animTree.Set("parameters/conditions/isMoving", hSpeed != 0);
        animTree.Set("parameters/conditions/isNotMoving", hSpeed == 0);
        animTree.Set("parameters/conditions/isOnGround", IsOnFloor());
        animTree.Set("parameters/Run/direction/blend_position", Math.Sign(hSpeed)); 
    }
    //Returns true when the player is in the air
    public bool AmIFlying(){
        return !IsOnFloor();
    }
    /*
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
    */
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