using Godot;
using System;

public class PlayerCombatantStateAirborne : CombatantState
{
    public override CombatantState Process(Combatant player, float delta){
        player.vSpeed -= player.data.GetFloat("gravity");
        player.hSpeed = Math.Sign(player.hSpeed) * (Math.Abs(player.hSpeed) - player.data.GetFloat("airDrag"));
        player.MoveAndSlide(new Vector3(player.hSpeed, player.vSpeed,0), Vector3.Up);
        if(!player.AmIFlying()){
            return new PlayerCombatantStateGround();  
        }
        return null;
    }
}
