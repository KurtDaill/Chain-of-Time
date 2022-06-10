using System;
using Godot;

public class CombatantStateStandby : CombatantState {

    public override void Enter(Combatant combatant, CombatantState lastState)
    {
        //TODO Add Travel to 
    }

    public override CombatantState Process(Combatant combatant){
        return null;
    }
}