using Godot;
using System;

public class PlayerCombatantStateSlide : PlayerCombatantState
{
   public override PlayerCombatantState Process(PlayerCombatant player){ //TODO: Make Hitbox match sprite during slides
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
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        return null;
   }

    public override void HandleAnimationTransition(PlayerCombatant player)
    {
        string animation = player.GetAnimatedSprite().Animation;
        if(animation == "Slide Start"){          
            player.rightFace = (player.hSpeed >= 0);
            player.SetAnim("Slide");
        }
    }

    public override void Enter(PlayerCombatant player, PlayerCombatantState lastState)
    {
        player.rightFace = (player.hSpeed >= 0);
        player.SetAnim("Slide Start");
    }
}
