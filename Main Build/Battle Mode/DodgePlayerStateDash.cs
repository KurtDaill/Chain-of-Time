using Godot;
using System;

public class DodgePlayerStateDash : DodgePlayerState
{
    public override DodgePlayerState Process(DodgePlayer player)
    {
        return null;
    }

    public override void Enter(DodgePlayer player){
        if(Input.IsActionPressed("ui_left")){ //Turn this into an "on enter" in dashing
                player.hSpeed -= player.dashBoost;  
            }else{
                player.hSpeed += player.dashBoost;
            }
    }
}
