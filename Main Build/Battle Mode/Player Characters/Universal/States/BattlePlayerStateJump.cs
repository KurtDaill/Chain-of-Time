using Godot;
using System;

public class PlayerCombatantStateJump : PlayerCombatantState
{
    /*
        The jump state is designed for use by other state when the player begins to jump
        The Airborne state can be transitioned to in cases where the player isn't jumping, so cannot include the "-= jumpforce" line
        This state preserves the design principle that changes between states should only include returning a new state object.
    */
    public override PlayerCombatantState Process(PlayerCombatant player)
    {
        return new PlayerCombatantStateAirborne();
    }

    public override void Enter(PlayerCombatant player, PlayerCombatantState lastState)
    {
        player.setSprite("Jump");
        player.rightFace = (player.hSpeed >= 0);
        player.vSpeed -= player.jumpForce;
        player.setNewHitbox("Standing Box");   
    }
}
