using System;
using Godot;

public class CombatantStatePain : CombatantState {

    private int minimumFrames = 20;

    private int framesPassed = 0;

    private int damageTaken;

    
    public CombatantStatePain(Combatant parent, Vector3 netKnockback, int damage,  int minimumFrames = 20){
        this.minimumFrames = minimumFrames;
        parent.hSpeed = netKnockback.x;
        parent.vSpeed = netKnockback.y;
        //TODO Damage Numbers
        damageTaken = damage;
    }

    public CombatantStatePain(){}

    public override void Enter(Combatant combatant, CombatantState lastState)
    {
        //TODO Add hit react animations
    }
    

    public override CombatantState Process(Combatant parent){
        /*
        Play Pain Animation
        If you hit the ground : If the minimum time has been met : Goto State Standby
        */
        framesPassed ++;
        parent.hSpeed = Math.Sign(parent.hSpeed) * Math.Max(0, (Math.Abs(parent.hSpeed) - parent.knockbackDrag));
        parent.MoveAndSlide(new Vector3(parent.hSpeed, parent.vSpeed,0));
        if(parent.AmIFlying()){
            parent.vSpeed += parent.knockbackGravity;
            return null;
        }
        else if (framesPassed >= minimumFrames){
            parent.vSpeed = 0;  
            return new CombatantStateStandby();
        }
        
        return null;
    }
}