using Godot;
using System;

public class PlayerCombatantStateAirborne : CombatantState
{
    public override CombatantState Process(Combatant player){
        player.vSpeed -= player.gravity;
        player.hSpeed = Math.Sign(player.hSpeed) * (Math.Abs(player.hSpeed) - player.airDrag);
        player.MoveAndSlide(new Vector3(player.hSpeed, player.vSpeed,0), Vector3.Up);
        if(!player.AmIFlying()){
            return new PlayerCombatantStateGround();  
        }
        return null;
    }
}
