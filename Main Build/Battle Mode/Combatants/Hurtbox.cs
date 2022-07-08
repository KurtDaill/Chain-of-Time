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
        if(box.CheckForImmune(parent)) return;
        parent.TakeDamage(box.GetDamage(), box.GetKnockback());
        box.LogHitCombatant(parent);
        GD.Print("Hurtbox hit registered on " + parent.Name + " : " + box.GetDamage() + " Damage.");
    }
}
