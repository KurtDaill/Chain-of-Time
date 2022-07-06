using Godot;
using System;

public class PlayerCombatantStateSlide : CombatantState
{
   public override CombatantState Process(Combatant combatant, float delta){ //TODO: Make Hitbox match sprite during slides
        combatant.hSpeed = Math.Sign(combatant.hSpeed) * Math.Max((Math.Abs(combatant.hSpeed) - combatant.data.GetFloat("slideDrag")), 0);
	    if(combatant.hSpeed == 0){
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
        combatant.MoveAndSlide(new Vector3(combatant.hSpeed, combatant.vSpeed,0));
        return null;
   }

    public override void Enter(Combatant combatant, CombatantState lastState)
    {
        //player.facing = Math.Sign(player.hSpeed);
        combatant.animSM.Travel("Slide Start");
    }

    public override void Exit(Combatant combatant){
        combatant.animSM.Travel("Slide End");
    }
}
