using System;
using Godot;

public partial class StatusTaunting : OnUpkeepStatus
{
    public override void _Ready(){
        name = "Taunt";
        animation = "Taunt";
        startingDuration = 3;
        remainingDuration = startingDuration;
        
        /*
            Flags:
                0 = StatusFlinch Animation is complete;
        */
        flagsRequiredToComplete = new bool[1];
    }
}