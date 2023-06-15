using Godot;
using System;
using static BattleUtilities;

public partial class PlayerSkill : PlayerAbility
{
    protected string skillType = "Attack";
	protected AbilityAlignment align = AbilityAlignment.Normal;

    public string GetSkilType(){
		return skillType;
	}
	public AbilityAlignment GetAbilityAlignment(){
		return align;
	}

} 
