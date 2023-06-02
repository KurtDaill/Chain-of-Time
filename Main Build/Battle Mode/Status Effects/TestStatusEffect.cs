using System;
using Godot;

public partial class TestStatusEffect : OnUpkeepStatus
{
    public override void _Ready(){
        name = "TestStatusEffect";
        animation = "StatusFlinch";
        defaultStartingDuration = 3;
        remainingDuration = defaultStartingDuration;
    }
    public override void Activate(int phase){
        base.Activate(phase);
    }
}