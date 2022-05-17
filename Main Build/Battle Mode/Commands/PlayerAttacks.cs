using Godot;
using System;
using System.Linq;

public class PlayerAttacks : BattleCommand
{
    PlayerCombatant playerCharacter;
    EnemyCombatant[] targets = new EnemyCombatant[3];
    int[] damagePerTarget = new int[3]{0,0,0};

    public PlayerAttacks(PlayerCombatant pc){
        this.playerCharacter = pc;
    }

    public override void Enter(Battle parent)
    {
        base.Enter(parent);
        parent.AddCommand(new EnemyAttacks());
        playerCharacter.SetState(new PlayerCombatantStateGround());
    }
    public override void Execute()
    {
        var areEnemiesInPain = false; //Is used to detect whether any enemy is in a pain state. 
                                     //We can't advance to the next command unitl all enemies are out of their pain state
        foreach(EnemyCombatant enemy in parent.activeCombatants.OfType<EnemyCombatant>()){
            if(enemy.DodgeBehaviour()){
                areEnemiesInPain = true;
            }
        }

        if(playerCharacter.MoveAndAttack(targets, damagePerTarget)){
            if(!areEnemiesInPain) parent.NextCommand();//Pause to Wait for all enemies to finish pain state!
        }
    }

    public override void Undo()
    {
        throw new NotImplementedException();
    }
}
