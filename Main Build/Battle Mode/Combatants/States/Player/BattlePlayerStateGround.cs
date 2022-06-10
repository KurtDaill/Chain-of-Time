using Godot;
using System;

public class PlayerCombatantStateGround : CombatantState {

    public override CombatantState Process(Combatant combatant){
        PlayerCombatant player = (PlayerCombatant) combatant; //TODO tidy this up?
        if(Input.IsActionPressed("ui_down")){
            if(Math.Abs(player.hSpeed) > 0) return new PlayerCombatantStateSlide();
            return new PlayerCombatantStateCrouch();
        }

        if(Input.IsActionPressed("ui_caps")){ //Start Dashing
            return new PlayerCombatantStateDash();
        }

        if(Input.IsActionJustPressed("ui_up")){
            return new PlayerCombatantStateJump();
        }

        if(Input.IsActionPressed("ui_right")){
            player.hSpeed = player.runSpeed;
        }else if(Input.IsActionPressed("ui_left")){
            player.hSpeed = -player.runSpeed;
        }else{
            player.hSpeed = Math.Sign(player.hSpeed) * Math.Max((Math.Abs(player.hSpeed) - player.footDrag), 0);
        }

        if(player.AmIFlying()) player.vSpeed -= player.gravity;
        else player.vSpeed = 0;
        player.MoveAndSlide(new Vector3(player.hSpeed, player.vSpeed,0), Vector3.Up);
        return null;
    }

    public override void Enter(Combatant player, CombatantState lastState)
    {
        string lastStateName = lastState.GetType().Name;
        //player.setNewHitbox("Standing Box");   
    }
}
