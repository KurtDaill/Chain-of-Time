using Godot;
using System;

public class EnemyAttacks : BattleCommand {

    public BattlePlayer target;

    public override void Enter(Battle parent){
        this.parent = parent;
        target = (BattlePlayer) parent.activeCombatants[0]; 
        //TODO:
        //Run the enemies logic to decide what attacks to throw
        //Might add in other Enemy Attack Commands if there are multiple targets
    }
    public override void Execute()
    {
        //Process the Player's Movement
        target.DefensiveMovement();
        /*
            TODO: Process the Enemies Attacks
        */
    }

    public override void Undo()
    {
        throw new NotImplementedException();
    }
}