using Godot;
using System;
using System.Threading.Tasks;
using static ItemUtilities;

public partial class IronsoulTablet : ConsumableItem
{

    [Export]
    int maxDrain = 100;
    Timer firstEffectTimer;
    public override void _Ready(){
        base._Ready();
        onCharacterAnimation = "DrinkPotion";
        firstEffectTimer = new Timer();
        this.AddChild(firstEffectTimer);
    }

    public async override Task Consume(PlayerCombatant user, Combatant[] targets){
        int drain = Math.Min(user.GetSP(), maxDrain);
        user.ChargeSP(drain); // TODO Make an animation for SP drain that gets called up here.
        firstEffectTimer.Start(1);
        await ToSignal(firstEffectTimer, Timer.SignalName.Timeout);
        user.Heal(drain);
        this.QueueFree();
        return;
    }
}