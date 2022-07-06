using Godot;
using System;

public class BattleCommandEnemyAttacks : BattleCommand {

    public PlayerCombatant target;

    public override void Enter(Battle parent, bool dual = false){
        base.Enter(parent, dual);

        target = (PlayerCombatant) parent.activeCombatants[0]; 
        target.SetState(new PlayerCombatantStateGround());
        //foreach(EnemyCombatant comm in parent.activeCombatants){
        var en = (EnemyCombatant) parent.activeCombatants[3];
        en.DecideAbility();
        //}
    }
    public override void Execute(float delta, Battle parent)
    {
        var readyToExit = true;
        //Process the Player's Movement
        target.Move(delta); 
        //Process the Enemies Action
            var en = (EnemyCombatant) parent.activeCombatants[3];
            if(en.ProcessAbility(delta)) readyToExit = false;
        if(readyToExit){
            parent.AddCommand(new BattlefieldCleanUp(true));
            parent.AddCommand(new PlayerMenuSelection());
            parent.NextCommand();
        }
    }

    public override void Exit()
    {
        target.SetState(new CombatantStateStandby());
    }

    public override void Undo()
    {
        throw new NotImplementedException();
    }
}