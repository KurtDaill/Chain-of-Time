using Godot;
using System;

public class PlayerCombatantStateDash : PlayerCombatantState
{
    public override PlayerCombatantState Process(PlayerCombatant player)
    {
        player.hSpeed = Math.Sign(player.hSpeed) * (Math.Abs(player.hSpeed) - player.dashDrag);
        if(Input.IsActionJustPressed("ui_down")){
            return new PlayerCombatantStateSlide();
        }
        if(Math.Abs(player.hSpeed) <= player.runSpeed){
            if(player.amIFlying()){ //if the player is airborne
                return new PlayerCombatantStateAirborne();
            }else{
                return new PlayerCombatantStateGround();
            }
        }
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        player.rightFace = (player.hSpeed >= 0);
        return null;
    }

    public override void Enter(PlayerCombatant player, PlayerCombatantState lastState){
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
