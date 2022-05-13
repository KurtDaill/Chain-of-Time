using Godot;
using System;

public class Hurtbox : Area2D
{
    Combatant parent;
    public override void _Ready()
    {
        base._Ready();
        parent = (Combatant) GetParent();
    }
    public void OnHurtboxAreaEntered(Hitbox box){
        //TODO Add calculation for knockback?
        parent.TakeDamage(box.GetDamage(), box.GetKnockback());
        GD.Print("Hurtbox hit registered");
    }
}
