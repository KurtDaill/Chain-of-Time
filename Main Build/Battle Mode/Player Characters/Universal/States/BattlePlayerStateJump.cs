using Godot;
using System;

public class PlayerCombatantStateJump : CombatantState
{
    /*
        The jump state is designed for use by other state when the player begins to jump
        The Airborne state can be transitioned to in cases where the player isn't jumping, so cannot include the "-= jumpforce" line
        This state preserves the design principle that changes between states should only include returning a new state object.
    */
    public override CombatantState Process(Combatant player)
    {
        return new PlayerCombatantStateAirborne();
    }

    public override void Enter(Combatant combatant, CombatantState lastState)
    {
        PlayerCombatant player = (PlayerCombatant) combatant;
        player.SetAnim("Jump");
        player.rightFace = (player.hSpeed >= 0);
        player.vSpeed -= player.jumpForce;
        player.setNewHitbox("Standing Box");   
    }
}
