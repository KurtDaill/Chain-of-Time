using Godot;
using System;

public class PlayerCombatantStateDash : CombatantState
{
    PlayerCombatant player;
    public override CombatantState Process(Combatant combatant)
    {
        player = (PlayerCombatant) combatant;
        player.hSpeed = Math.Sign(player.hSpeed) * (Math.Abs(player.hSpeed) - player.dashDrag);
        if(Input.IsActionJustPressed("ui_down")){
            return new PlayerCombatantStateSlide();
        }
        if(Math.Abs(player.hSpeed) <= player.runSpeed){
            if(player.AmIFlying()){ //if the player is airborne
                return new PlayerCombatantStateAirborne();
            }else{
                return new PlayerCombatantStateGround();
            }
        }
        player.MoveAndSlide(new Vector3(player.hSpeed, player.vSpeed,0));
        player.rightFace = (player.hSpeed >= 0);
        return null;
    }

    public override void Enter(Combatant combatant, CombatantState lastState){
        player = (PlayerCombatant) combatant;
        if(Input.IsActionPressed("ui_left") || player.hSpeed < 0){ //Turn this into an "on enter" in dashing
                player.hSpeed -= player.dashBoost;
            }else{
                player.hSpeed += player.dashBoost;
        }
        player.animSM.Travel("Dash");
        player.dashing = true;
        //player.setNewHitbox("Standing Box");   
    }

    public override void Exit(){
        player.animSM.Travel("Run");
        player.dashing = false;
    }
}
