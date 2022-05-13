using Godot;
using System;

public class EnemyCombatantStateExit : EnemyCombatantState {

    public override EnemyCombatantState Process(EnemyCombatant enemy){
        throw new UnhandledExitStateExcpetion();    
    }
}