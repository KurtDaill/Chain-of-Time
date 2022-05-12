using Godot;
using System;

public class CatoStateAttackTwo : PlayerCombatantState {

    int[] damageRecord;
    EnemyCombatant[] targets;

    Hitbox hitbox = null;
    Godot.PackedScene hitboxResource = (PackedScene) GD.Load("res://Battle Mode/Player Characters/Cato/Cato Atk 2 Hitbox.tscn");
    PlayerCombatant player;

    public override void Enter(PlayerCombatant player, PlayerCombatantState lastState)
    {
        base.Enter(player, lastState);
        player.setSprite("Attack Two");
        this.player = player;
    }
    public CatoStateAttackTwo(int[] dr, EnemyCombatant[] tar){
        this.damageRecord = dr;
        this.targets = tar;
    }

    public override PlayerCombatantState Process(PlayerCombatant player){
            if(player.GetAnimatedSprite().Frame == 2){
                if(hitbox == null){
                    hitbox = (Hitbox) hitboxResource.Instance();
                    player.AddChild(hitbox);
                    hitbox.SetDamage((int) Mathf.Round(player.strength * 1.25F));
                }
            }
            return null;
    }

    public override void Exit()
    {
        hitbox.QueueFree();
    }

    public override void HandleAnimationTransition(PlayerCombatant player)
    {
        //Attack Two animation has finished, and the player hasn't given any valid input, we exit the attack sequence.
        player.SetState(new PlayerCombatantStateExit());
    }
}