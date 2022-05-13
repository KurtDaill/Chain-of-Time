using Godot;
using System;

public class EnemyCombatant : Combatant
{
    protected AnimatedSprite sprite;

    protected EnemyCombatantState state = new EnemyCombatantStateExit();

    //TODO add painState function to all Combatants?
    //Is true while the enemy is stunned and in a knockback state
    public bool inPainState = false;
    protected EnemyAttack[] attacksKnown;

    public override void _Ready()
    {
        hitbox = (Area2D) GetNode("Hitbox");
        if(hitbox == null){
            throw new NotImplementedException();
        }
        sprite = (AnimatedSprite) GetNode("AnimatedSprite");
        if(sprite == null){
            throw new NotImplementedException();
        }
    }

    public override void _Process(float delta)
    {
        //TODO implement a minimum painState timer?
        //TODO Standardize Enemy Animation Control
        if(inPainState){
            
        }
    }

    //Ran every frame while a player is attacking!
    public virtual void DodgeBehaviour(){

    }

    public override int TakeDamage(int incomingDamage, Vector2 knockback){
        //TODO Damage Numbers, Hit Animation, knockback
        int dmg = base.TakeDamage(incomingDamage, knockback);
        GD.Print(Name + " hit! : " + dmg + " Damage Dealt!");
        inPainState = true;
        return dmg;
    }

    
}
