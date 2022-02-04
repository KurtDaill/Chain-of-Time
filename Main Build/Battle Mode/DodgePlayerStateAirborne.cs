using Godot;
using System;

public class DodgePlayerStateAirborne : DodgePlayerState
{
    public override DodgePlayerState Process(DodgePlayer player){
        player.vSpeed += player.gravity;
        player.hSpeed = Math.Sign(player.hSpeed) * (Math.Abs(player.hSpeed) - player.airDrag);
        if(player.vSpeed >= 0 && player.GetAnimPlayer().CurrentAnimation == "Air Up") player.setAnim("Air Transition"); //TODO Fix this conditional
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        if(!player.amIFlying()){
            return new DodgePlayerStateGround();  
        }
        return null;
    }

    public override void HandleAnimationTransition(DodgePlayer player){
        string animation = player.GetAnimPlayer().CurrentAnimation;
        player.rightFace = (player.hSpeed >= 0);
        if(animation == "Jump"){
            player.setAnim("Air Up");
        }else if(animation == "Air Up" && player.vSpeed > 0){ //This conditional is useful redundancy
            player.setAnim("Air Trans");
        }else if(animation == "Air Trans"){
            player.setAnim("Air Down");
        }
        player.setNewHitbox("Standing Box");   
    }
}
