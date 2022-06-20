using Godot;
using System;

public class PlayerCombatantStateCrouch : CombatantState
{
    PlayerCombatant player;
    public override CombatantState Process(Combatant player)
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
        player = (PlayerCombatant) combatant;
        player.crouching = true;
        player.animSM.Travel("Crouch");
    }

    public override void Exit(){
        player.crouching = false;
    }
}
