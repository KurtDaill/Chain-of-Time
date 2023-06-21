using Godot;
using System;
using static BattleUtilities;

public partial class PlayerSkill : PlayerAbility
{
	[Export]
	protected int spCost = -1;
    protected string skillType = "Attack";
	protected AbilityAlignment align = AbilityAlignment.Normal;

    public string GetSkilType(){
		return skillType;
	}
	public AbilityAlignment GetAbilityAlignment(){
		return align;
	}
	public int GetSPCost(){
		return spCost;
	}


} 
