using Godot;
using System;
using System.Threading.Tasks;
using static ItemUtilities;

public partial class WhiteStoneSigil : ConsumableItem
{
    Timer firstEffectTimer;
    public override void _Ready(){
        base._Ready();
        targeting = BattleUtilities.TargetingLogic.SingleTargetEnemy;
        onCharacterAnimation = "DrinkPotion"; //TODO Change Animation Here
        firstEffectTimer = new Timer();
        this.AddChild(firstEffectTimer);
    }

    public async override Task Consume(PlayerCombatant user, Combatant[] targets){
        targets[0].TakeDamage(999); //Need to have some cool effect here, but that's beside the point.
        this.QueueFree();
        return;
    }
}