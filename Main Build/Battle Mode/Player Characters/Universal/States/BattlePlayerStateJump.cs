using Godot;
using System;

public class BattlePlayerStateJump : BattlePlayerState
{
    /*
        The jump state is designed for use by other state when the player begins to jump
        The Airborne state can be transitioned to in cases where the player isn't jumping, so cannot include the "-= jumpforce" line
        This state preserves the design principle that changes between states should only include returning a new state object.
    */
    public override BattlePlayerState Process(BattlePlayer player)
    {
        return new BattlePlayerStateAirborne();
    }

    public override void Enter(BattlePlayer player, BattlePlayerState lastState)
    {
        player.setSprite("Jump");
        player.rightFace = (player.hSpeed >= 0);
        player.vSpeed -= player.jumpForce;
        player.setNewHitbox("Standing Box");   
    }
}
