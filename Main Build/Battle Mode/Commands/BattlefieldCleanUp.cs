using Godot;
using System;


public class BattlefieldCleanUp : BattleCommand
{
    //ASSUMPTION: All characters are already on the ground
    //ASSUMPTION: All characters should be a part of the battlefield order
    private Vector3[] originalPos = new Vector3[6];
    private float[] startingDistance = new float[6];

    private int[] interpDelta = new int[]{0,0,0,0,0,0};

    private float ticker = 0;


    public override void Enter(Battle parent)
    {
        /*
            Assigns Original positions to interpolate from
            Original Position is taken from objects in order
        */
        for(int i = 0; i < parent.activeCombatants.GetLength(0); i++){
            if(parent.activeCombatants[i] != null){
                originalPos[i] = parent.activeCombatants[i].GlobalTransform.origin;
                //startingDistance[i] = (parent.battleSpots[i].GlobalTransform.origin - originalPos[i]).Length();
            }
        }
    }
    public override void Execute(float delta, Battle parent)
    {
        ticker += delta;
        if(ticker >= 1) ticker = 1;
        /*
            Interpolates between original and targetPos logarithmically while moving the camera at the same rate

            foreach activeCombatant
                D = D + (C * (Current Distance - 1/2 Original Distance ))
                N += D
                interpolate(original, battleSpot, N)
        */

        for(int i = 0; i < parent.activeCombatants.GetLength(0); i++){
            if(parent.activeCombatants[i] != null){
                parent.activeCombatants[i].Transform = parent.activeCombatants[i].Transform.InterpolateWith(parent.battleSpots[i].Transform, delta * 2.5F);
            }
        }
        //TODO Replace this Test Script
    }
    public override void Undo()
    {
        return;
    }
}
