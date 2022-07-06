using Godot;
using System;

public class PlayerCombatantStateCrouch : CombatantState
{
    public override CombatantState Process(Combatant player, float delta)
    {
        if(!Input.IsActionPressed("ui_down")){
            return new PlayerCombatantStateGround();
        }
        if(Input.IsActionJustPressed("ui_up")){
            return new PlayerCombatantStateJump();
        }
        return null;
    }

    public override void Enter(Combatant combatant, CombatantState lastState)
    {
        combatant.data.SetBool("crouching", true);
        combatant.animSM.Travel("Crouch");
    }

    public override void Exit(Combatant combatant){
        combatant.data.SetBool("crouching", false);    
    }
}
