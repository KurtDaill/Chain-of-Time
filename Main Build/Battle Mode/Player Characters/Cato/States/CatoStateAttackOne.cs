using Godot;
using System;

public class CatoStateAttackOne : PlayerCombatantState {

    int[] damageRecord;
    EnemyCombatant[] targets;

    Area2D hurtbox = null;

    public override void Enter(PlayerCombatant player, PlayerCombatantState lastState)
    {
        base.Enter(player, lastState);
        player.setSprite("Attack One");
    }
    public void acceptAttackData(int[] dr, EnemyCombatant[] tar){
        this.damageRecord = dr;
        this.targets = tar;
    }

    public override PlayerCombatantState Process(PlayerCombatant player){
            if(player.GetAnimatedSprite().Frame == 3){
                if(hurtbox == null){
                    //Instance Create Atk 1 Hurtbox
                }
                Godot.Collections.Array hitAreas = hurtbox.GetOverlappingAreas();
                for(int i = 0; i < hitAreas.Count; i++){
                    if(hitAreas[i] is EnemyCombatant){
                        if(Array.Exists<EnemyCombatant>(targets, l => l == hitAreas[i])){
                            int index = Array.FindIndex<EnemyCombatant>(targets, j => j == null);
                            targets[index] = (EnemyCombatant) hitAreas[i];
                            damageRecord[index] = targets[i].TakeDamage(player.strength);
                        }
                    }
                }
            }
            /*
            TODO
            1. Collision Object
            2. Check for Attack Two: Critical Hit checks
            3. Log Damage/Target
            */
        throw new NotImplementedException();
    }

    public override void HandleAnimationTransition(PlayerCombatant player)
    {
        //Attack One animation has finished, and the player hasn't given any valid input, we exit the attack sequence.
        player.SetState(new PlayerCombatantStateExit());
    }
}