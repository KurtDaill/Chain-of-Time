using Godot;
using System;
using System.Collections.Generic;

public class Hitbox : Area
{
    int damage;

    private List<Combatant> combatantsIveHit = new List<Combatant>(); 

    Vector3 knockback = Vector3.Zero;
    public override void _Ready()
    {

    }

    public void SetDamage(int damage){
        this.damage = damage;
    }

    public int GetDamage(){
        return damage;
    }

    public void SetKnockback(Vector3 knockback){
        this.knockback = knockback;
    }

    public Vector3 GetKnockback(){
        return knockback;
    }

    public virtual bool CheckForImmune(Combatant target){ //Called by hurtboxes before taking damage to check if this hitbox shouldn't damage that combatant.
    //Returns true if this target is immune 
        return combatantsIveHit.Contains(target);
    }

    public virtual void LogHitCombatant(Combatant  comm){
        if(combatantsIveHit.Contains(comm)) return;
        combatantsIveHit.Add(comm);
    }
}
