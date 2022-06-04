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

    int knockbackStrength = 100;
    PlayerCombatant player;

    public override void Enter(Combatant player, CombatantState lastState)
    {
        base.Enter(player, lastState);
        player.SetSprite("Attack One");
        this.player = (PlayerCombatant) player;
    }
    public CatoStateAttackOne(int[] dr, EnemyCombatant[] tar){
        this.damageRecord = dr;
        this.targets = tar;
    }

    public override CombatantState Process(Combatant combatant){
            CatoCombatant player = (CatoCombatant) combatant;
            if(player.GetAnimatedSprite().Frame == 2){
                if(hitbox == null){
                    hitbox = (Hitbox) hitboxResource.Instance();
                    player.AddChild(hitbox);
                    hitbox.SetDamage(player.strength);
                    hitbox.SetKnockback(new Vector2(player.facing, -.5F) * knockbackStrength);
                    return null;
                }
            }
            if(Input.IsActionJustPressed("com_atk") && !attackLocked){
                if(frameCounter < player.secondAttackTimer){
                    attackLocked = true;
                }
                else{
                    return new CatoStateAttackTwo(damageRecord, targets);
                }
            }
            frameCounter++;
                /*
                var hitAreas = hurtbox.GetOverlappingAreas();
                for(int i = 0; i < hitAreas.Count; i++){
                    if(hitAreas[i] is EnemyCombatant){
                        if(Array.Exists<EnemyCombatant>(targets, l => l == hitAreas[i])){
                            var index = Array.FindIndex<EnemyCombatant>(targets, j => j == null);
                            targets[index] = (EnemyCombatant) hitAreas[i];
                            damageRecord[index] = targets[i].TakeDamage(player.strength);
                        }
                    }
                }
                */
            
            return null;
    }

    public override void Exit()
    {
        hitbox.QueueFree();
    }

    public override void HandleAnimationTransition(Combatant player)
    {
        //Attack One animation has finished, and the player hasn't given any valid input, we exit the attack sequence.
        player.SetState(new CombatantStateExit());
    }
}