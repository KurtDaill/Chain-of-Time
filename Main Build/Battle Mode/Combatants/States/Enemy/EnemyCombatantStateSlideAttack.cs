using Godot;
using System;
using static AbilityUtilities;

public class EnemyCombatantStateSlideAttack : CombatantState
{
    private float acceleration, topSpeed, attackTime;
    public EnemyCombatantStateSlideAttack(float speed, float accel, float time){
        acceleration = accel;
        topSpeed = speed;
        attackTime = time;
    }
    public override void Enter(Combatant combatant, CombatantState lastState)
    {
        CheckForRequiredAnimations(combatant, new string[]{"Slide Start, Slide, Slide Stop"});
        combatant.animSM.Travel("Slide Start");
        combatant.DisableCombatantCollisions();
        base.Enter(combatant, lastState);
    }
    public override CombatantState Process(Combatant parent, float delta)
    {
        if(parent.animSM.GetCurrentNode() == "Slide Start") return null;
        parent.hSpeed -= acceleration;
        if(parent.hSpeed > topSpeed){
            parent.hSpeed = topSpeed;
        }
        parent.MoveAndSlide(new Vector3(parent.hSpeed, 0, 0));
        attackTime -= delta;
        if(attackTime < 0){
            parent.animSM.Travel("Slide Stop");
            return new CombatantStateStandby();
        }
        return null;
    }

    public override void Exit(Combatant combatant)
    {
        combatant.EnableCombatantCollisions();
        base.Exit(combatant);
    }
}
