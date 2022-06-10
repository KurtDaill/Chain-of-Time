using Godot;
using System;

public class PlayerCombatantStateCrouch : CombatantState
{
    public override CombatantState Process(Combatant player)
    {
        if(!Input.IsActionJustPressed("ui_down")){
            return new PlayerCombatantStateGround();
        }
        return null;
    }

    public override void Enter(Combatant player, CombatantState lastState)
    {
        //Change Hitbox to crouch version
    }
}
