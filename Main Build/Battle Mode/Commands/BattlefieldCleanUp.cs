using Godot;
using System;


public class BattlefieldCleanUp : BattleCommand
{
    //ASSUMPTION: All characters are already on the ground
    //ASSUMPTION: All characters should be a part of the battlefield order
    private Vector3[] originalPos = new Vector3[6];
    private float[] startingDistance = new float[6];

    private int[] interpDelta = new int[]{0,0,0,0,0,0};

    private float terminator = 2;

    private bool playerTurn = false; //Was it the player's turn before this cleanup command was executed

    private float speed = 2.5F;

    private PlayerMenuSelection playerMenu;

    Boolean facePlayer; //Should the camera face towards the player at the end of this transition

    public BattlefieldCleanUp(bool facingPlayer){
        facePlayer = facingPlayer;
    }

    public override void Enter(Battle parent, bool dual = false)
    {
        base.Enter(parent, dual);
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
        if(facePlayer)parent.camera.InterpolateToTransform(parent.camera.baseTransform.Rotated(Vector3.Up, 0.0872665F), speed, terminator);
        else parent.camera.InterpolateToTransform(parent.camera.baseTransform.Rotated(Vector3.Up, -0.0872665F), speed, terminator);

        if(parent.PeakCommand() is PlayerMenuSelection){
            playerMenu = (PlayerMenuSelection) parent.PeakCommand();
            playerMenu.Enter(parent);
            runningDual = true;
        }
    }
    public override void Execute(float delta, Battle parent)
    {
        /*
            Interpolates between original and targetPos logarithmically while moving the camera at the same rate

            foreach activeCombatant
                D = D + (C * (Current Distance - 1/2 Original Distance ))
                N += D
                interpolate(original, battleSpot, N)
        */
        for(int i = 0; i < parent.activeCombatants.GetLength(0); i++){
            if(parent.activeCombatants[i] != null){
                parent.activeCombatants[i].Transform = parent.activeCombatants[i].Transform.InterpolateWith(parent.battleSpots[i].Transform, delta * speed);
            }
        }
        if(playerMenu != null){
            playerMenu.Execute(delta, parent);
        }
        terminator -= delta;
        if(terminator <= 0){
            if(runningDual){
                runningDual = false;
                parent.NextCommand(false);
            }
            else
            {
                parent.NextCommand();
            }
        }
    }
    public override void Undo()
    {
        return;
    }
}
