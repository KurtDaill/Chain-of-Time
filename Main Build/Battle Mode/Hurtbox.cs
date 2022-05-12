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
        parent.TakeDamage(box.GetDamage());
        GD.Print("Hurtbox hit registered");
    }
}
