using System;
using Godot;

public class EnemyCombatantStatePain : EnemyCombatantState {

    private int minimumFrames = 20;
    public EnemyCombatantStatePain(int minimumFrames){
        this.minimumFrames = minimumFrames;
    }

    public EnemyCombatantStatePain(){}

    public override EnemyCombatantState Process(EnemyCombatant enemy){
        /*
        Play Pain Animation
        If you hit the ground : If the minimum time has been met : Goto State Standby
        */
        
        return null;
    }
}