using Godot;
using System;

public class BattlePlayerStateDash : BattlePlayerState
{
    public override BattlePlayerState Process(BattlePlayer player)
    {
        player.hSpeed = Math.Sign(player.hSpeed) * (Math.Abs(player.hSpeed) - player.dashDrag);
        if(Input.IsActionJustPressed("ui_down")){
            return new BattlePlayerStateSlide();
        }
        if(Math.Abs(player.hSpeed) <= player.runSpeed){
            if(player.amIFlying()){ //if the player is airborne
                return new BattlePlayerStateAirborne();
            }else{
                return new BattlePlayerStateGround();
            }
        }
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        player.rightFace = (player.hSpeed >= 0);
        return null;
    }

    public override void Enter(BattlePlayer player, BattlePlayerState lastState){
        if(Input.IsActionPressed("ui_left") || player.hSpeed < 0){ //Turn this into an "on enter" in dashing
                player.hSpeed -= player.dashBoost;
                player.setSprite("Dash");  
            }else{
                player.hSpeed += player.dashBoost;
                player.setSprite("Dash");  
        }
        player.setNewHitbox("Standing Box");   
    }
}
