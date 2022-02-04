using Godot;
using System;

public class DodgePlayerStateCrouch : DodgePlayerState
{
    public override DodgePlayerState Process(DodgePlayer player)
    {
        if(!Input.IsActionJustPressed("ui_down")){
            return new DodgePlayerStateGround();
        }
        return null;
    }

    public override void Enter(DodgePlayer player, DodgePlayerState lastState)
    {
        player.setNewHitbox("Crouch Box");
        player.setAnim("Crouch");
    }
}
