using Godot;
using System;

public partial class OnUpkeepStatus : StatusEffect
{
    public override void Begin(){
        ShowNotification();
        //Bypass Animation Check
        running = true;
        //EmitSignal(CombatAction.SignalName.ActionComplete);
    }
}
