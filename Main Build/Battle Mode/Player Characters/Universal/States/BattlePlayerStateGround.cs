using Godot;
using System;

public class PlayerCombatantStateGround : CombatantState {

    public override CombatantState Process(Combatant combatant){
        PlayerCombatant player = (PlayerCombatant) combatant; //TODO tidy this up
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

        if(player.AmIFlying()) player.vSpeed += player.gravity;
        else player.vSpeed = 0;
        //Animations (Running while moving, Idle while standing) TODO make this conditional less stupid
        if(player.hSpeed == 0 && player.GetAnimatedSprite().Animation != "Landing"){
            player.setSprite("Idle");
        }else if(player.hSpeed != 0 && player.GetAnimatedSprite().Animation != "Slide Recovery" && player.GetAnimatedSprite().Animation != "Landing"){
            player.setSprite("Run");
        }
        if(player.GetAnimatedSprite().Animation != "Landing") player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        player.rightFace = (player.hSpeed >= 0);
        return null;
    }

    public override void HandleAnimationTransition(Combatant player){
        string animation = player.GetAnimatedSprite().Animation;
        if(animation == "Landing"){
            player.setSprite("Idle");
        }
        if(animation == "Slide Recovery"){
            player.setSprite("Run");
            player.GetAnimatedSprite().Frame = 1;
        }
    }

    public override void Enter(Combatant player, CombatantState lastState)
    {
        string lastStateName = lastState.GetType().Name;
        if(lastStateName == "PlayerCombatantStateSlide"){
            player.setSprite("Slide Recovery");
        }else if(lastStateName == "PlayerCombatantStateAirborne"){
            player.setSprite("Landing");
        }
        player.setNewHitbox("Standing Box");   
    }
}
