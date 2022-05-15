using Godot;
using System;

//End Turn should always be followed by a battlefield clean up:
//TODO: Add an exception? for this ^
public class EndTurn : BattleCommand
{
    public override void Execute()
    {
        throw new NotImplementedException();
    }
    public override void Undo()
    {
        throw new NotImplementedException();
    }
}
