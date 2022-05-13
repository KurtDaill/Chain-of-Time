using Godot;
using System;

public class CatoStateAttackTwo : CombatantState {

    int[] damageRecord;
    EnemyCombatant[] targets;

    Hitbox hitbox = null;
    Godot.PackedScene hitboxResource = (PackedScene) GD.Load("res://Battle Mode/Player Characters/Cato/Cato Atk 2 Hitbox.tscn");
    PlayerCombatant player;

    private float knockbackStrength = 150;

    public override void Enter(Combatant player, CombatantState lastState)
    {
        base.Enter(player, lastState);
        player.setSprite("Attack Two");
        this.player = (PlayerCombatant) player;
    }
    public CatoStateAttackTwo(int[] dr, EnemyCombatant[] tar){
        this.damageRecord = dr;
        this.targets = tar;
    }

    public override CombatantState Process(Combatant player){
            if(player.GetAnimatedSprite().Frame == 2){
                if(hitbox == null){
                    hitbox = (Hitbox) hitboxResource.Instance();
                    player.AddChild(hitbox);
                    hitbox.SetDamage((int) Mathf.Round(this.player.strength * 1.25F));
                    hitbox.SetKnockback(new Vector2(player.facing, 0) * knockbackStrength);
                }
            }
            return null;
    }

    public override void Exit()
    {
        hitbox.QueueFree();
    }

    public override void HandleAnimationTransition(Combatant player)
    {
        //Attack Two animation has finished, and the player hasn't given any valid input, we exit the attack sequence.
        player.SetState(new CombatantStateExit());
    }
}