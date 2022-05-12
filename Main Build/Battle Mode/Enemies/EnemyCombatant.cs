using Godot;
using System;

public class EnemyCombatant : Combatant
{
    protected AnimatedSprite sprite;

    protected EnemyAttack[] attacksKnown;

    public override void _Ready()
    {
        hitbox = (Area2D) GetNode("Hitbox");
        if(hitbox == null){
            throw new NotImplementedException();
        }
    }

    public override int TakeDamage(int incomingDamage){
        //TODO Damage Numbers
        int dmg = base.TakeDamage(incomingDamage);
        GD.Print(Name + " hit! : " + dmg + " Damage Dealt!");
        return dmg;
    }
}
