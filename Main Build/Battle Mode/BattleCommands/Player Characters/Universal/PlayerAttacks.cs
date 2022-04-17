using Godot;
using System;

public class PlayerAttacks : BattleCommand
{
    PlayerCombatant playerCharacter;
    EnemyCombatant[] targets = new EnemyCombatant[3];
    int[] damagePerTarget = new int[3]{0,0,0};

    PlayerAttacks(PlayerCombatant pc){
        this.playerCharacter = pc;
    }

    public override void Enter(Battle parent)
    {
        base.Enter(parent);
        parent.AddCommand(new EnemyAttacks());
    }
    public override void Execute()
    {
        if(playerCharacter.MoveAndAttack(targets, damagePerTarget)){
            parent.NextCommand();
        }
    }

    public override void Undo()
    {
        throw new NotImplementedException();
    }
}
