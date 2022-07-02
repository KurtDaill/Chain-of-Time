using Godot;
using System;
using System.Linq;

public class PlayerAttacks : BattleCommand
{
    EnemyCombatant[] targets = new EnemyCombatant[3];
    PlayerCombatant playerCharacter;
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
    public override void Execute(float delta, Battle parent)
    {
        var areEnemiesReady = true; //Is used to detect whether all enemies are ready to exit the attack sequence 
        foreach(EnemyCombatant enemy in parent.activeCombatants.OfType<EnemyCombatant>()){
            if(enemy.DodgeBehaviour()){
                areEnemiesReady = false;
            }
        }

        if(playerCharacter.MoveAndAttack(targets, damagePerTarget)){
            if(areEnemiesReady) parent.NextCommand();//Pause to Wait for all enemies to finish pain state!
        }
    }

    public override void Undo()
    {
        throw new NotImplementedException();
    }
}
