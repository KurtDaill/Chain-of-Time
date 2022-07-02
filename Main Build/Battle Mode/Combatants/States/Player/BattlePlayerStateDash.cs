using Godot;
using System;

public class PlayerCombatantStateDash : CombatantState
{
    public override CombatantState Process(Combatant combatant)
    {
        combatant.hSpeed = Math.Sign(combatant.hSpeed) * (Math.Abs(combatant.hSpeed) - combatant.data.GetFloat("dashDrag"));
        if(Input.IsActionJustPressed("ui_down")){
            return new PlayerCombatantStateSlide();
        }
        if(Math.Abs(combatant.hSpeed) <= combatant.data.GetFloat("runSpeed")){
            if(combatant.AmIFlying()){ //if the combatant is airborne
                return new PlayerCombatantStateAirborne();
            }else{
                return new PlayerCombatantStateGround();
            }
        }
        combatant.MoveAndSlide(new Vector3(combatant.hSpeed, combatant.vSpeed,0));
        return null;
    }

    public override void Enter(Combatant combatant, CombatantState lastState){
        if(Input.IsActionPressed("ui_left") || combatant.hSpeed < 0){ //Turn this into an "on enter" in dashing
                combatant.hSpeed -= combatant.data.GetFloat("dashBoost");
            }else{
                combatant.hSpeed += combatant.data.GetFloat("dashBoost");
        }
        combatant.animSM.Travel("Dash");
        combatant.data.SetBool("dashing", true);
        //combatant.setNewHitbox("Standing Box");   
    }

    public override void Exit(Combatant combatant){
        combatant.animSM.Travel("Run");
        combatant.data.SetBool("dashing", false);
    }
}
