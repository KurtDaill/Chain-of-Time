
using Godot;
using System;

public class PlayerCombatantStateExit : PlayerCombatantState {
    public override PlayerCombatantState Process(PlayerCombatant player)
    {
        throw new UnhandledExitStateExcpetion();
    }
}

public class UnhandledExitStateExcpetion : Exception
{
    public UnhandledExitStateExcpetion(){}

    public UnhandledExitStateExcpetion(string message): base(message){}

    public UnhandledExitStateExcpetion(string message, Exception inner): base(message, inner){}
}
