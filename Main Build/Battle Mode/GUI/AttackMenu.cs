using Godot;
using System;

public class AttackMenu : BattleMenu
{
    public override void OnOpen(PMPlayerCharacter character, PMBattle caller)
    {
        base.OnOpen(character, caller);
    }

    public override PMPlayerAbility HandleInput(MenuInput input, PMPlayerCharacter character, PMBattle caller){
        if(input == MenuInput.Back){
            parentGUI.ChangeMenu(0, character, caller);
            return null;
        }else if(input == MenuInput.Select){
            var ability = character.GetBasicAttack();
            ability.SetTargets(new PMCharacter[]{caller.PositionLookup(PMBattleUtilities.BattlePos.EnemyOne)});//TODO make conform with selection functions
            return ability;
        }
        return null;
    }
}
