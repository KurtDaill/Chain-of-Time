using Godot;
using System;

public class CatoStateAttackOne : CombatantState {
    //TODO: Should transition to something else rather than looping when complete
    int[] damageRecord;
    EnemyCombatant[] targets;

    Hitbox hitbox = null;
    Godot.PackedScene hitboxResource = (PackedScene) GD.Load("res://Battle Mode/Combatants/Player Characters/Cato/Cato Atk 1 Hitbox.tscn");

    bool attackLocked = false;
    int frameCounter = 0;

    PlayerCombatant player;

    public override void Enter(Combatant player, CombatantState lastState)
    {
        base.Enter(player, lastState);
        this.player = (PlayerCombatant) player;
        player.animSM.Travel("Attack 1");
    }

    public CatoStateAttackOne(int[] dr, EnemyCombatant[] tar){
        this.damageRecord = dr;
        this.targets = tar;
    }

    public override CombatantState Process(Combatant combatant){
            CatoCombatant player = (CatoCombatant) combatant;
            if(Input.IsActionJustPressed("com_atk") && !attackLocked){
                if(frameCounter < player.secondAttackTimer){
                    attackLocked = true;
                }
                else{
                    return new CatoStateAttackTwo(damageRecord, targets);
                }
            }
            frameCounter++;
            
            return null;
    }

    public override void Exit()
    {
        
    }

    public override void HandleAnimationTransition(Combatant player)
    {
        //Attack One animation has finished, and the player hasn't given any valid input, we exit the attack sequence.
        player.SetState(new CombatantStateExit());
    }
}