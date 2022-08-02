using Godot;
using System;

public class BattleCommandEnemyAttacks : BattleCommand {

    public PlayerCombatant target;
    private float timer = 1;

    public override void Enter(Battle parent, bool dual = false){
        base.Enter(parent, dual);
        parent.positionManager.InterpolateToDefaultPositions(3F, 1F);
        target = (PlayerCombatant) parent.activeCombatants[0]; 
        target.SetState(new PlayerCombatantStateGround());
        //foreach(EnemyCombatant comm in parent.activeCombatants){
        //}
    }
    public override void Execute(float delta, Battle parent)
    {
        if(timer > 0){
            timer -= delta;
            if(timer < 0){
                var foo = (EnemyCombatant) parent.activeCombatants[3];
                foo.DecideAbility();
            }
            return;
        }
        var readyToExit = true;
        //Process the Player's Movement
        target.Move(delta); 
        //Process the Enemies Action
            var en = (EnemyCombatant) parent.activeCombatants[3];
            if(en.ProcessAbility(delta)) readyToExit = false;
        if(readyToExit){
            //parent.AddCommand(new BattlefieldCleanUp(false));
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