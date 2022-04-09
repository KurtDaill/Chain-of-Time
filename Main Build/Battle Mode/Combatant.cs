using Godot;
using System;

public abstract class Combatant : KinematicBody2D {
    private int hitPoints;
    private int maxHP;

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

    public void recoverHP(int heal){
        if(heal <= 0){
            throw new NegativeHealingException();
        }
        hitPoints += heal;
        if(hitPoints >= maxHP){
            hitPoints = maxHP;
        }
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