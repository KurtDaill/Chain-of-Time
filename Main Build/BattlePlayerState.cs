using Godot;
using System;
using System.Diagnostics;

public abstract class BattlePlayerState : CombatantState
{
    public BattlePlayerState() : base(){
        Debug.Assert(self is BattlePlayer);
    }
}
