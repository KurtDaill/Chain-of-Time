using System;
using Godot;

public partial class StatusTaunting : OnUpkeepStatus
{
    public override void _Ready(){
        name = "Taunt";
        animation = "Taunt";
        defaultStartingDuration = 3;
        remainingDuration = defaultStartingDuration;
        
        /*
            Flags:
                0 = StatusFlinch Animation is complete;
        */
        flagsRequiredToComplete = new bool[1];
    }
}