using Godot;
using System;
public class PlayerCombatant : Combatant
{
    protected PlayerCombatantSkillState[] preparedSkills = new PlayerCombatantSkillState[4];
    public virtual void Move(float delta)
    {
        if(state == null){
            throw new ArgumentNullException();
        }
        
        newState = state.Process(this, delta);
        if(newState != null) SetState(newState);
        newState = null;
    }

    public virtual bool MoveAndAttack(EnemyCombatant[] targets, int[] damageRecord, float delta){
        return false;
    } //TODO Refactor

    public PlayerCombatantSkillState GetSkill(int i){
        return preparedSkills[i];
    }

    public PlayerCombatantSkillState[] GetSkills(){
        return preparedSkills;
    }
}