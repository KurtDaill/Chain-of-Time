using Godot;
using System;

public class BattlePlayerStateSlide : BattlePlayerState
{
   public override BattlePlayerState Process(BattlePlayer player){ //TODO: Make Hitbox match sprite during slides
        player.hSpeed = Math.Sign(player.hSpeed) * Math.Max((Math.Abs(player.hSpeed) - player.slideDrag), 0);
	    if(player.hSpeed == 0){
			return new BattlePlayerStateCrouch();
		}
        /*
        if(Input.IsActionPressed("ui_up")){
            player.vSpeed -= player.jumpForce;
            return new BattlePlayerStateJump();
        }
        */
        if(!Input.IsActionPressed("ui_down")){
            return new BattlePlayerStateGround();
        }
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        return null;
   }

    public override void HandleAnimationTransition(BattlePlayer player)
    {
        string animation = player.GetAnimatedSprite().Animation;
        if(animation == "Slide Start"){          
            player.rightFace = (player.hSpeed >= 0);
            player.setSprite("Slide");
        }
    }

    public override void Enter(BattlePlayer player, BattlePlayerState lastState)
    {
        player.rightFace = (player.hSpeed >= 0);
        player.setSprite("Slide Start");
    }
}
