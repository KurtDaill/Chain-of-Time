using System;
using Godot;

public partial class TestStatusEffect : OnUpkeepStatus
{
    public override void _Ready(){
        name = "TestStatusEffect";
        animation = "StatusFlinch";
        startingDuration = 3;
        remainingDuration = startingDuration;
        
        /*
            Flags:
                0 = StatusFlinch Animation is complete;
        */
        flagsRequiredToComplete = new bool[1];
    }
    public override void AnimationTrigger(int phase){
        base.AnimationTrigger(phase);
    }
}