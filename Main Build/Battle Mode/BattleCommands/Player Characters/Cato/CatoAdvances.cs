using Godot;
using System;

public class CatoAdvances : BattleCommand
{
    //Indicates the distance from the target that Cato should stand before his next animation
    Vector2 distanceFromTarget;
    Vector2 startingPosition

    public CatoAdvances(Vector2 dft, Vector2 sp){
        distanceFromTarget = dft;
        startingPosition = sp;
    }
    public override void Execute()
    {
        //Move Cato <Move Speed> per frame along a line between his staring position and the target position

    }
    public override void Undo()
    {
        throw new NotImplementedException();
    }
}
