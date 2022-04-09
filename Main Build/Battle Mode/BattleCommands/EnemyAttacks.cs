using Godot;
using System;

public class EnemyAttacks : BattleCommand {

    public BattlePlayer target;

    public override void Enter(Battle parent){
        this.parent = parent;
        target = (BattlePlayer) parent.activeCombatants[0]; 
        target.SetState(new BattlePlayerStateGround());
        //TODO:
        //Run the enemies logic to decide what attacks to throw
        //Might add in other Enemy Attack Commands if there are multiple targets
        /*
            if(parent.activeCombatants[3] != null){
                (EnemyCombatant) parent.activeCombatants[3].DecideAttack(parent)
            }
            repeat for 4 & 5
        */
    }
    public override void Execute()
    {
        //Process the Player's Movement
        target.ExecuteDefensiveMovement();
        /*
            TODO: Process the Enemies Attacks
            if(parent.activeCombatants[3] != null && parent.activeCombatants[3].attacking){
                parent.activeCombatants[3].ExecuteAttack();
            }
            repeat for 4 & 5
        */
    }

    public override void Exit()
    {
        target.SetState(null);
    }

    public override void Undo()
    {
        throw new NotImplementedException();
    }
}