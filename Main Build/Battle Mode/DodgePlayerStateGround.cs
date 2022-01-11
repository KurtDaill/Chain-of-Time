using Godot;
using System;

public class DodgePlayerStateGround : DodgePlayerState {
    public override DodgePlayerState Process(DodgePlayer player){

        if(Input.IsActionPressed("ui_down")){
            return new DodgePlayerStateSlide();
        }

        if(Input.IsActionPressed("ui_caps")){ //Start Dashing
            return new DodgePlayerStateDash();
        }

        if(Input.IsActionJustPressed("ui_up")){
            player.vSpeed -= player.jumpForce;
            return new DodgePlayerStateAirborne();
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
        if(player.hSpeed == 0 && player.GetAnimatedSprite().Animation != "Landing"){
            player.setSprite("Idle");
        }else if(player.hSpeed != 0){
            player.setSprite("Run", Math.Sign(player.hSpeed));
        }
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        return null;
    }

    public override void HandleAnimationTransition(DodgePlayer player){
        string animation = player.GetAnimatedSprite().Animation;
        if(animation == "Landing"){
            player.setSprite("Idle");
        }
    }    
}
