using Godot;
using System;

public class DodgePlayerStateJump : DodgePlayerState
{
    /*
        The jump state is designed for use by other state when the player begins to jump
        The Airborne state can be transitioned to in cases where the player isn't jumping, so cannot include the "-= jumpforce" line
        This state preserves the design principle that changes between states should only include returning a new state object.
    */
    public override DodgePlayerState Process(DodgePlayer player)
    {
        return new DodgePlayerStateAirborne();
    }

    public override void Enter(DodgePlayer player, DodgePlayerState lastState)
    {
        player.setSprite("Jump");
        player.vSpeed -= player.jumpForce;
    }
}
