using Godot;
using System;

public abstract class PlayerCombatant : Combatant {
    //Called every frame where the player is allowed to control their movement
    public abstract void ExecuteDefensiveMovement();
}