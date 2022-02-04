using Godot;
using System;

public class DodgePlayerStateDash : DodgePlayerState
{
    public override DodgePlayerState Process(DodgePlayer player)
    {
        player.hSpeed = Math.Sign(player.hSpeed) * (Math.Abs(player.hSpeed) - player.dashDrag);
        if(Input.IsActionJustPressed("ui_down")){
            return new DodgePlayerStateSlide();
        }
        if(Math.Abs(player.hSpeed) <= player.runSpeed){
            if(player.amIFlying()){ //if the player is airborne
                return new DodgePlayerStateAirborne();
            }else{
                return new DodgePlayerStateGround();
            }
        }
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        player.rightFace = (player.hSpeed >= 0);
        return null;
    }

    public override void Enter(DodgePlayer player, DodgePlayerState lastState){
        if(Input.IsActionPressed("ui_left") || player.hSpeed < 0){ //Turn this into an "on enter" in dashing
                player.hSpeed -= player.dashBoost;
                player.setAnim("Dash");  
            }else{
                player.hSpeed += player.dashBoost;
                player.setAnim("Dash");  
        }
        player.setNewHitbox("Standing Box");   
    }
}
