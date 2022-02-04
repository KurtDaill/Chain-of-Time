using Godot;
using System;

public class DodgePlayerStateSlide : DodgePlayerState
{
   public override DodgePlayerState Process(DodgePlayer player){ //TODO: Make Hitbox match sprite during slides
        player.hSpeed = Math.Sign(player.hSpeed) * Math.Max((Math.Abs(player.hSpeed) - player.slideDrag), 0);
	    if(player.hSpeed == 0){
			return new DodgePlayerStateCrouch();
		}
        /*
        if(Input.IsActionPressed("ui_up")){
            player.vSpeed -= player.jumpForce;
            return new DodgePlayerStateJump();
        }
        */
        if(!Input.IsActionPressed("ui_down")){
            return new DodgePlayerStateGround();
        }
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        return null;
   }

    public override void HandleAnimationTransition(DodgePlayer player)
    {
        string animation = player.GetAnimPlayer().CurrentAnimation;
        if(animation == "Slide Start"){          
            player.rightFace = (player.hSpeed >= 0);
            player.setAnim("Slide");
        }
    }

    public override void Enter(DodgePlayer player, DodgePlayerState lastState)
    {
        player.rightFace = (player.hSpeed >= 0);
        player.setAnim("Slide Start");
    }
}
