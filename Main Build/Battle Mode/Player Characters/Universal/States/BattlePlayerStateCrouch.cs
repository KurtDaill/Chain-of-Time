using Godot;
using System;

public class PlayerCombatantStateCrouch : PlayerCombatantState
{
    public override PlayerCombatantState Process(PlayerCombatant player)
    {
        if(!Input.IsActionJustPressed("ui_down")){
            return new PlayerCombatantStateGround();
        }
        return null;
    }

    public override void Enter(PlayerCombatant player, PlayerCombatantState lastState)
    {
        player.setNewHitbox("Crouch Box");
        player.setSprite("Crouch");
    }
}
