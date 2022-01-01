using Godot;
using System;

public abstract class BattleState
{
 
    /* 
    The BattleState is passed to Battle Manager, and is used to store the behaviours assocaiated with it's various states .
    The big trick is to allow states to hand Battle Manager their own new states when appropriate: new enemies, menu's, and player abilites require no 
    direct changes to Battle Manager: only their own new states to hand it
    */
    public abstract BattleState HandleInput(BattleManager bm); //Used to handle Player Input
    public abstract BattleState Process(BattleManager bm); //Used to handle game state behaviour independant of the player
}
