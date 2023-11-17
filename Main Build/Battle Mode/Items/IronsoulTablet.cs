using Godot;
using System;
using System.Threading.Tasks;
using static ItemUtilities;

public partial class IronsoulTablet : ConsumableItem
{

    [Export]
    int maxDrain = 100;
    Timer myTimer;
    public override void _Ready(){
        base._Ready();
        onCharacterAnimation = "DrinkPotion";
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        myTimer = new Timer();
        this.AddChild(myTimer);
    }

    public async override Task Consume(PlayerCombatant user, Combatant[] targets){
        int drain = Math.Min(user.GetSP(), maxDrain);
        user.ChargeSP(drain); // TODO Make an animation for SP drain that gets called up here.
        myTimer.Start(1);
        await ToSignal(myTimer, Timer.SignalName.Timeout);
        user.Heal(drain);
        this.QueueFree();
        return;
    }
}