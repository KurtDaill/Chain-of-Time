using Godot;
using System;

public class CatoStateAttackOne : PlayerCombatantState {

    int[] damageRecord;
    EnemyCombatant[] targets;

    Hitbox hitbox = null;
    Godot.PackedScene hitboxResource = (PackedScene) GD.Load("res://Battle Mode/Player Characters/Cato/Cato Atk 1 Hitbox.tscn");

    bool attackLocked = false;
    int frameDelayToAttackTwo = 25;
    int frameCounter = 0;
    PlayerCombatant player;

    public override void Enter(PlayerCombatant player, PlayerCombatantState lastState)
    {
        base.Enter(player, lastState);
        player.setSprite("Attack One");
        this.player = player;
    }
    public CatoStateAttackOne(int[] dr, EnemyCombatant[] tar){
        this.damageRecord = dr;
        this.targets = tar;
    }

    public override PlayerCombatantState Process(PlayerCombatant player){
            if(player.GetAnimatedSprite().Frame == 2){
                if(hitbox == null){
                    hitbox = (Hitbox) hitboxResource.Instance();
                    player.AddChild(hitbox);
                    hitbox.SetDamage(player.strength);
                    return null;
                }
            }
            if(Input.IsActionJustPressed("com_atk") && !attackLocked){
                if(frameCounter < frameDelayToAttackTwo){
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

    public override void HandleAnimationTransition(PlayerCombatant player)
    {
        //Attack One animation has finished, and the player hasn't given any valid input, we exit the attack sequence.
        player.SetState(new PlayerCombatantStateExit());
    }
}