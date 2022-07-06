using Godot;
using System;

public class CatoStateAttackTwo : CombatantState {

    int[] damageRecord;
    EnemyCombatant[] targets;

    Hitbox hitbox = null;
    Godot.PackedScene hitboxResource = (PackedScene) GD.Load("res://Battle Mode/Combatants/Player Characters/Cato/Cato Atk 2 Hitbox.tscn");
    PlayerCombatant player;

    private float knockbackStrength = 150;

    //TODO Implement Critical Hits
    public override void Enter(Combatant player, CombatantState lastState)
    {
        base.Enter(player, lastState);
        this.player = (PlayerCombatant) player;
    }
    public CatoStateAttackTwo(int[] dr, EnemyCombatant[] tar){
        this.damageRecord = dr;
        this.targets = tar;
    }

    public override CombatantState Process(Combatant player, float delta){
            /*if(player.GetAnimatedSprite().Frame == 2){
                if(hitbox == null){
                    hitbox = (Hitbox) hitboxResource.Instance();
                    player.AddChild(hitbox);
                    hitbox.SetDamage((int) Mathf.Round(this.player.strength * 1.25F));
                    hitbox.SetKnockback(new Vector3(player.facing, 0,0) * knockbackStrength);
                }
            }TODO: Replace This!*/
            return null;
    }

    public override void Exit(Combatant combatant)
    {
        hitbox.QueueFree();
    }
}