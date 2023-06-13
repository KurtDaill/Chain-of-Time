using System;
using Godot;

public partial class TestStatusEffect : OnUpkeepStatus
{
    public override void _Ready(){
        name = "TestStatusEffect";
        animation = "StatusFlinch";
        defaultStartingDuration = 3;
        remainingDuration = defaultStartingDuration;
        
        /*
            Flags:
                0 = StatusFlinch Animation is complete;
        */
        flagsRequiredToComplete = new bool[1];
    }
    public override void Activate(int phase){
        base.Activate(phase);
    }

}