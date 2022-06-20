using Godot;
using System;

public class PlayerCombatantStateSlide : CombatantState
{
    PlayerCombatant player;
   public override CombatantState Process(Combatant combatant){ //TODO: Make Hitbox match sprite during slides
        PlayerCombatant player = (PlayerCombatant) combatant;
        player.hSpeed = Math.Sign(player.hSpeed) * Math.Max((Math.Abs(player.hSpeed) - player.slideDrag), 0);
	    if(player.hSpeed == 0){
			return new PlayerCombatantStateCrouch();
		}
        /*
        if(Input.IsActionPressed("ui_up")){
            player.vSpeed -= player.jumpForce;
            return new PlayerCombatantStateJump();
        }
        */
        if(!Input.IsActionPressed("ui_down")){
            return new PlayerCombatantStateGround();
        }
        player.MoveAndSlide(new Vector3(player.hSpeed, player.vSpeed,0));
        return null;
   }

    public override void Enter(Combatant combatant, CombatantState lastState)
    {
        player = (PlayerCombatant) combatant;
        //player.facing = Math.Sign(player.hSpeed);
        player.animSM.Travel("Slide Start");
    }

    public override void Exit(){
        player.animSM.Travel("Slide End");
    }
}
