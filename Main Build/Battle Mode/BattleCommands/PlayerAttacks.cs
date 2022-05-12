using Godot;
using System;

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
        //TODO Make a more flexible solution for setting the initial state
        playerCharacter.SetState(new PlayerCombatantStateGround());
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
