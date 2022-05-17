using Godot;
using System;

public class PlayerCombatantStateSlide : CombatantState
{
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
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        return null;
   }

    public override void HandleAnimationTransition(Combatant player)
    {
        string animation = player.GetAnimatedSprite().Animation;
        if(animation == "Slide Start"){          
            player.facing = Math.Sign(player.hSpeed);
            player.SetSprite("Slide");
        }
    }

    public override void Enter(Combatant player, CombatantState lastState)
    {
        player.facing = Math.Sign(player.hSpeed);
        player.SetSprite("Slide Start");
    }
}
