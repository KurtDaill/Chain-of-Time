using Godot;
using System;

public class DodgePlayerStateGround : DodgePlayerState {
    public override DodgePlayerState Process(DodgePlayer player){

        if(Input.IsActionPressed("ui_down")){
            if(Math.Abs(player.hSpeed) > 0) return new DodgePlayerStateSlide();
            return new DodgePlayerStateCrouch();
        }

        if(Input.IsActionPressed("ui_caps")){ //Start Dashing
            return new DodgePlayerStateDash();
        }

        if(Input.IsActionJustPressed("ui_up")){
            return new DodgePlayerStateJump();
        }

        if(Input.IsActionPressed("ui_right")){
            player.hSpeed = player.runSpeed;
        }else if(Input.IsActionPressed("ui_left")){
            player.hSpeed = -player.runSpeed;
        }else{
            player.hSpeed = Math.Sign(player.hSpeed) * Math.Max((Math.Abs(player.hSpeed) - player.footDrag), 0);
        }

        if(player.amIFlying()) player.vSpeed += player.gravity;
        else player.vSpeed = 0;
        //Animations (Running while moving, Idle while standing) TODO make this conditional less stupid
        if(player.hSpeed == 0 && player.GetAnimPlayer().CurrentAnimation != "Landing"){
            player.setAnim("Idle");
        }else if(player.hSpeed != 0 && player.GetAnimPlayer().CurrentAnimation != "Slide Recovery" && player.GetAnimPlayer().CurrentAnimation != "Landing"){
            player.setAnim("Run");
        }
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        player.rightFace = (player.hSpeed >= 0);
        return null;
    }

    public override void HandleAnimationTransition(DodgePlayer player){
        string animation = player.GetAnimPlayer().CurrentAnimation;
        if(animation == "Landing"){
            player.setAnim("Idle");
        }
        if(animation == "Slide Recovery"){
            player.setAnim("Run");
            player.GetAnimPlayer().Seek(0.72F);
        }
    }

    public override void Enter(DodgePlayer player, DodgePlayerState lastState)
    {
        string lastStateName = lastState.GetType().Name;
        GD.Print(lastStateName);
        if(lastStateName == "DodgePlayerStateSlide"){
            player.setAnim("Slide Recovery");
        }else if(lastStateName == "DodgePlayerStateAirborne"){
            player.setAnim("Landing");
        }
        player.setNewHitbox("Standing Box");   
    }
}
