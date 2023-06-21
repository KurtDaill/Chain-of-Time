using System;
using Godot;

public partial class EmpoweredStatus : StatusEffect{
    [Export]
    int damageBonus;
    [Export]
    bool onlyEffectsBasicAttacks;

    public int GetBonusDamage(bool isSpecialAttack){
        if(onlyEffectsBasicAttacks && isSpecialAttack){
            return 0;
        }
        return damageBonus;
    }
}