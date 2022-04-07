using Godot;
using System;

public class BattlePlayerStateCrouch : BattlePlayerState
{
    public override BattlePlayerState Process(BattlePlayer player)
    {
        if(!Input.IsActionJustPressed("ui_down")){
            return new BattlePlayerStateGround();
        }
        return null;
    }

    public override void Enter(BattlePlayer player, BattlePlayerState lastState)
    {
        player.setNewHitbox("Crouch Box");
        player.setSprite("Crouch");
    }
}
