using Godot;
using System;
using System.Threading.Tasks;
using static ItemUtilities;

public partial class OrcishFireBrew : ConsumableItem
{

    [Export]
    int spGained;
    [Export]
    int damageTaken;
    Timer firstEffectTimer, secondEffectTimer;
    public override void _Ready(){
        base._Ready();
        onCharacterAnimation = "DrinkPotion";
        firstEffectTimer = new Timer();
        secondEffectTimer = new Timer();
        this.AddChild(firstEffectTimer);
        this.AddChild(secondEffectTimer);
    }

    public async override Task Consume(PlayerCombatant user, Combatant[] targets){
        user.GainSP(spGained);
        firstEffectTimer.Start(1);
        await ToSignal(firstEffectTimer, Timer.SignalName.Timeout);
        user.TakeDamage(damageTaken);
        secondEffectTimer.Start(1);
        await ToSignal(secondEffectTimer, Timer.SignalName.Timeout);
        this.QueueFree();
        return;
    }
}