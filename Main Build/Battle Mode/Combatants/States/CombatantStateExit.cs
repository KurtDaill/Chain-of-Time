
using Godot;
using System;

public class CombatantStateExit : CombatantState {
    public override CombatantState Process(Combatant combatant)
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
