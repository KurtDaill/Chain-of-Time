using Godot;
using System;


public class BattlefieldCleanUp : BattleCommand
{
    private bool readyForSlide = false;
    private float deltaT = 0;
    private Vector3[] originalPos = new Vector3[6];
    private Vector3[] targetPos = new Vector3[6];
    public override void Execute()
    {
        /*
            1. Return all combatants to their starting positions
                1. Check if All Combatants are either off screen or on the ground;
                2. Save All Combatants original position, and where they have to go.
                3. For Each Combatant: Lerp between their original and target positions over the next second
                4. Once everyone is at their target positions, end the command
            2. Destroy all projectiles and other trash in the scene
        */

        if(readyForSlide == false){        
            readyForSlide = true;
            foreach(Combatant comb in parent.activeCombatants){ //Moves all Combatants towards the ground
                if(comb.AmIFlying()){
                    readyForSlide = false;
                    comb.vSpeed += comb.gravity;
                    comb.MoveAndSlide(new Vector3(0, comb.vSpeed,0));
                }else{
                    comb.vSpeed = 0;
                }
            }
            
        } /*else{   //All Combatants should be on the ground
            if(!Array.Exists(originalPos, (x => x != null))){ //If every entry in original position is null...
                for(int i = 0; i < parent.activeCombatants.Length; i++){ //Populate both original and target position arrays
                    if(parent.activeCombatants[i] != null){ 
                        originalPos[i] = parent.activeCombatants[i].Position;
                        targetPos[i] = parent.battleSpots[i].Position;
                    }
                }
            }
            deltaT += parent.GetProcessDeltaTime();
            for(int i = 0; i < parent.activeCombatants.Length; i++){
                if(originalPos[i] != null){
                    parent.activeCombatants[i].Position = new Vector3(Mathf.Lerp(originalPos[i].x, targetPos[i].x, Mathf.Log(deltaT)),  parent.activeCombatants[i].Position.y,0);
                }
            }
        }
        */
    }
    public override void Undo()
    {
        return;
    }
}
