using Godot;
using System;

public class Hurtbox : Area
{
    Combatant parent;
    public override void _Ready()
    {
        base._Ready();
        parent = (Combatant) GetParent();
    }
    public void OnHurtboxAreaEntered(Hitbox box){
        parent.TakeDamage(box.GetDamage(), box.GetKnockback());
        GD.Print("Hurtbox hit registered");
    }
}
