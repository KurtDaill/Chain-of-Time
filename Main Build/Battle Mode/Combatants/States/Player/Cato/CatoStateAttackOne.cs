using Godot;
using System;

public class CatoStateAttackOne : CombatantState {
    //TODO: Should transition to something else rather than looping when complete
    int[] damageRecord;
    EnemyCombatant[] targets;

    bool attackLocked = false;
    int frameCounter = 0;

    PlayerCombatant player;

    public override void Enter(Combatant player, CombatantState lastState)
    {
        base.Enter(player, lastState);
        this.player = (PlayerCombatant) player;
        player.animSM.Start("Attack 1");
        player.hSpeed = 0;
    }

    public CatoStateAttackOne(int[] dr, EnemyCombatant[] tar){
        this.damageRecord = dr;
        this.targets = tar;
    }

    public override CombatantState Process(Combatant combatant, float delta){
            if(Input.IsActionJustPressed("com_atk") && !attackLocked){
                if(frameCounter < player.data.GetFloat("secondAttackTimer")){
                    attackLocked = true;
                }
                else{
                    return new CatoStateAttackTwo(damageRecord, targets);
                }
            }
            frameCounter++;
            if(combatant.animSM.GetCurrentNode() != "Attack 1"){
                combatant.animSM.Travel("Idle");
                return new CombatantStateStandby(true);
            }
            return null;
    }
}