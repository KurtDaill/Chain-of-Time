using System;
using Godot;

public class CombatantStateStandby : CombatantState {

    public override void Enter(Combatant combatant, CombatantState lastState)
    {
        combatant.hSpeed = 0;
        //TODO Add Travel to 
    }

    public override CombatantState Process(Combatant combatant){
        if(!combatant.IsOnFloor()){
            combatant.vSpeed -= combatant.data.GetFloat("gravity");
            combatant.MoveAndCollide(new Vector3(combatant.hSpeed, combatant.vSpeed, 0));
        }else{
            combatant.vSpeed = 0;
        }
        return null;
    }
}