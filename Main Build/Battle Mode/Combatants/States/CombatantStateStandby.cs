using System;
using Godot;

public class CombatantStateStandby : CombatantState {

    private bool lockedToIdle = false;

    public CombatantStateStandby(){}
    public CombatantStateStandby(bool idle){
        lockedToIdle = idle;
    }
    public override CombatantState Process(Combatant combatant, float delta){
        if(!combatant.IsOnFloor()){
            combatant.vSpeed -= combatant.data.GetFloat("gravity");
            combatant.MoveAndCollide(new Vector3(combatant.hSpeed, combatant.vSpeed, 0));
        }else{
            combatant.vSpeed = 0;
        }
        if(combatant.hSpeed != 0){
            combatant.hSpeed = Math.Sign(combatant.hSpeed) * Math.Min(0, (Math.Abs(combatant.hSpeed) - combatant.data.GetFloat("footDrag")));
        }
        if(lockedToIdle && combatant.animSM.GetCurrentNode() != "Idle"){
            combatant.animSM.Start("Idle");
        }
        return null;
    }
}