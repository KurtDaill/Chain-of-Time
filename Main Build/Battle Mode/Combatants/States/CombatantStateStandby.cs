using System;
using Godot;

public class CombatantStateStandby : CombatantState {

    public override void Enter(Combatant combatant, CombatantState lastState)
    {
        combatant.SetSprite("Idle");
    }

    public override CombatantState Process(Combatant combatant){
        return null;
    }
}