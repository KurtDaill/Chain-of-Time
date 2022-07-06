using Godot;
using System;
using System.Collections.Generic;

public abstract class Combatant : KinematicBody {
    protected int hitPoints;
    private int maxHP;
    protected int armor = 0;

    protected CombatantState state = new CombatantStateStandby();
    protected CombatantState newState;

    [Export]
    public string character = "DEFAULT";

    public AnimationTree animTree;
    public AnimationNodeStateMachinePlayback animSM;

    public List<Hitbox> hitBoxes = new List<Hitbox>();
    public CombatantData data = new CombatantData();

    public int facing = 1; //Equals 1 while facing right, -1 while facing left, used in calculations dependent on character's facing
    [Export]
	public float gravity = 9F;
    [Export]
    public float knockbackDrag = 3F;
    [Export]
    public float knockbackGravity = 3F;
	[Export]
	public float airDrag = 0.21F;
    [Export]
    public float knockbackResist = 0F; //On a scale of 0-1 how much do we reduce knockback inflicted on this character. 

    public float hSpeed = 0;
    public float vSpeed = 0;

    protected virtual void SetCombatantData(){
        data.SetFloat("gravity", gravity);
        data.SetFloat("knockbackDrag", knockbackDrag);
        data.SetFloat("knockbackGravity", knockbackGravity);
        data.SetFloat("airDrag", airDrag);
        data.SetFloat("knockbackResist", knockbackResist);
        data.SetBool("painState", false);
    }
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

    public virtual void updateAnimationTree(){
        animTree.Set("parameters/conditions/isFalling", vSpeed < 10);
        animTree.Set("parameters/conditions/isMoving", hSpeed != 0);
        animTree.Set("parameters/conditions/isNotMoving", hSpeed == 0);
        animTree.Set("parameters/conditions/isOnGround", IsOnFloor());
        animTree.Set("parameters/" + animSM.GetCurrentNode() +"/blend_position", facing);
        //var curNode = (AnimationNodeBlendSpace1D)animSM.GetCurrentNode();
        //curNode.SetBlendPointPosition(facing);
        //animTree.Set("parameters/Run/direction/blend_position", Math.Sign(hSpeed)); 
    }

    public void SetState(CombatantState newState){
        state.Exit(this);
        CombatantState temp = state;
        state = newState;
        state.Enter(this, temp);
    }

    public virtual bool ProcessAbility(float delta){ //Called by Battle Commands when a combatant should be executing an ability state (NPC attack, Spell Cast, etc.) Returns whether it's still executing that ability state
        var newState = state.Process(this, delta);
        if(newState != null){
            SetState(newState);
        }
        if(state is CombatantStateStandby){
            return false;
        }
        return true;
    }

    public virtual void spawnHitbox(String requestedHitbox){}

    public virtual void clearHitboxes(){ //TODO surface THIS function to animation players without rewritting it in each combatant
        foreach(Hitbox box in hitBoxes){
            box.QueueFree();
            hitBoxes.Remove(box);
        }
    }

    public virtual void DisableCombatantCollisions(){
        this.CollisionMask = 0x1;

    }
    public virtual void EnableCombatantCollisions(){
        this.CollisionMask = 0x31;
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