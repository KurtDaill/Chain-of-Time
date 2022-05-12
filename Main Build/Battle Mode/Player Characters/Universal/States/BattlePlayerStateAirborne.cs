using Godot;
using System;

public class PlayerCombatantStateAirborne : PlayerCombatantState
{
    public override PlayerCombatantState Process(PlayerCombatant player){
        player.vSpeed += player.gravity;
        player.hSpeed = Math.Sign(player.hSpeed) * (Math.Abs(player.hSpeed) - player.airDrag);
        if(player.vSpeed >= 0 && player.GetAnimatedSprite().Animation == "Air Up") player.setSprite("Air Transition"); //TODO Fix this conditional
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        if(!player.amIFlying()){
            return new PlayerCombatantStateGround();  
        }
        return null;
    }

    public override void HandleAnimationTransition(PlayerCombatant player){
        string animation = player.GetAnimatedSprite().Animation;
        player.rightFace = (player.hSpeed >= 0);
        if(animation == "Jump"){
            player.setSprite("Air Up");
        }else if(animation == "Air Up" && player.vSpeed > 0){ //This conditional is useful redundancy
            player.setSprite("Air Transition");
        }else if(animation == "Air Transition"){
            player.setSprite("Air Down");
        }
        player.setNewHitbox("Standing Box");   
    }
}
