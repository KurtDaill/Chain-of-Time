using Godot;
using System;

public class EnemyCombatant : Combatant
{
    
    public bool inPainState = false;
    protected EnemyAttack[] attacksKnown;

    public override void _Ready()
    {
        //hitbox = (Area2D) GetNode("Hitbox");
        //if(hitbox == null){
            //throw new NotImplementedException();
        //}
    }
    public override void _Process(float delta)
    {

    }

    //Ran every frame while a player is attacking!
    //Returns true if this enemy is in a pain state
    public virtual bool DodgeBehaviour(){
        var newState = state.Process(this);
        if(newState != null){
            SetState(newState);
        }
        return state is CombatantStatePain;
    }

    public override int TakeDamage(int incomingDamage, Vector3 knockback){
        int dmg = base.TakeDamage(incomingDamage, knockback);
        GD.Print(Name + " hit! : " + dmg + " Damage Dealt!");
        inPainState = true;
        return dmg;
    }
}
