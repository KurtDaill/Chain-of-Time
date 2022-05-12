using Godot;
using System;

public abstract class Combatant : KinematicBody2D {
    private int hitPoints;
    private int maxHP;

    protected int armor = 0;

    [Export]
	public float gravity = 9F;

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
    public virtual int TakeDamage(int incomingDamage){
        int damage = Math.Max(0, incomingDamage - armor);
        hitPoints -= damage;
        //TODO Damage Numbers and other effects
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
    public bool amIFlying(){
        if(GetSlideCount() == 0) return true;

        KinematicCollision2D kc = GetSlideCollision(0);
        if(kc.Collider != null) return false;
        return true;
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