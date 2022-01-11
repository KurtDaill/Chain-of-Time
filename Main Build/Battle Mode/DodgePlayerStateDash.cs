using Godot;
using System;

public class DodgePlayerStateDash : DodgePlayerState
{
    public override DodgePlayerState Process(DodgePlayer player)
    {
        player.hSpeed = Math.Sign(player.hSpeed) * (Math.Abs(player.hSpeed) - player.dashDrag);
        if(Math.Abs(player.hSpeed) <= player.runSpeed){
            if(player.amIFlying()){ //if the player is airborne
                return new DodgePlayerStateAirborne();
            }else{
                return new DodgePlayerStateGround();
            }
        }
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        return null;
    }

    public override void Enter(DodgePlayer player){
        if(Input.IsActionPressed("ui_left") || player.hSpeed < 0){ //Turn this into an "on enter" in dashing
                player.hSpeed -= player.dashBoost;
                player.setSprite("Dash", -1);  
            }else{
                player.hSpeed += player.dashBoost;
                player.setSprite("Dash");  
            }
    }
}
