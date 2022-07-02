using Godot;
using System;

public class PlayerCombatantStateGround : CombatantState {

    public override CombatantState Process(Combatant combatant){
        if(Input.IsActionPressed("ui_down")){
            if(Math.Abs(combatant.hSpeed) > 0) return new PlayerCombatantStateSlide();
            return new PlayerCombatantStateCrouch();
        }

        if(Input.IsActionPressed("ui_caps")){ //Start Dashing
            return new PlayerCombatantStateDash();
        }

        if(Input.IsActionJustPressed("ui_up")){
            return new PlayerCombatantStateJump();
        }

        if(Input.IsActionPressed("ui_right")){
            combatant.hSpeed = combatant.data.GetFloat("runSpeed");
        }else if(Input.IsActionPressed("ui_left")){
            combatant.hSpeed = -combatant.data.GetFloat("runSpeed");
        }else{
            combatant.hSpeed = Math.Sign(combatant.hSpeed) * Math.Max((Math.Abs(combatant.hSpeed) - combatant.data.GetFloat("footDrag")), 0);
        }

        if(combatant.AmIFlying()) combatant.vSpeed -= combatant.gravity;
        else combatant.vSpeed = 0;
        combatant.MoveAndSlide(new Vector3(combatant.hSpeed, combatant.vSpeed,0), Vector3.Up);
        return null;
    }

    public override void Enter(Combatant player, CombatantState lastState)
    {
        string lastStateName = lastState.GetType().Name;
        //player.setNewHitbox("Standing Box");   
    }
}
