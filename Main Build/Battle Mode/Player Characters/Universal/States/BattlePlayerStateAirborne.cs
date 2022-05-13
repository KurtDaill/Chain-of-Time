using Godot;
using System;

public class PlayerCombatantStateAirborne : CombatantState
{
    public override CombatantState Process(Combatant player){
        player.vSpeed += player.gravity;
        player.hSpeed = Math.Sign(player.hSpeed) * (Math.Abs(player.hSpeed) - player.airDrag);
        if(player.vSpeed >= 0 && player.GetAnimatedSprite().Animation == "Air Up") player.setSprite("Air Transition"); //TODO Fix this conditional
        player.MoveAndSlide(new Vector2(player.hSpeed, player.vSpeed));
        if(!player.AmIFlying()){
            return new PlayerCombatantStateGround();  
        }
        return null;
    }

    public override void HandleAnimationTransition(Combatant player){
        string animation = player.GetAnimatedSprite().Animation;
        player.facing = Math.Sign(player.hSpeed);
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
