using System;
using Godot;

public partial class TestSkill : PlayerSkill
{
    public override void _Ready(){
        base._Ready();
        name = "TestSkill";
        animation = "SpellCast";
        align = BattleUtilities.AbilityAlignment.Magic;
        skillType = "Spell"; 
        AbilityTargetingLogic = BattleUtilities.TargetingLogic.Self;
    }
}