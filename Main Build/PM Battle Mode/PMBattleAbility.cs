using Godot;
using System;
using static PMBattleUtilities;
public class  PMBattleAbility : Node
{
    [Export(PropertyHint.Enum)]
    public static AbilityAlignment align = AbilityAlignment.Normal;

    [Export(PropertyHint.Enum)]
    public static TargetRule targeting = TargetRule.Self;

    public void DealDamage(int damage, int target = -1, int al = -1){
        TargetRule localTarget = ConvertTarget(target);
        AbilityAlignment localAlign = ConvertAlignment(al);
    }

    public void Heal(int heal, int target = -1, int al = -1){
        TargetRule localTarget = ConvertTarget(target);
        AbilityAlignment localAlign = ConvertAlignment(al);
    }

    public void InflictStatus(int status, int duration = 1, int target = -1, int al = -1){
        TargetRule localTarget = ConvertTarget(target);
        AbilityAlignment localAlign = ConvertAlignment(al);
    }   



    private TargetRule ConvertTarget(int target){
        if(target == -1){
            return targeting;
        }
        return (TargetRule) target;
    }

    private AbilityAlignment ConvertAlignment(int alignment){
        if(alignment == -1){
            return align;
        }
        return (AbilityAlignment) alignment;
    }
}

