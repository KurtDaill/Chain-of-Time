using Godot;
using System;

public class AttackMenu : BattleMenu
{
    public override void OnOpen()
    {
        base.OnOpen();
    }

    public override PMPlayerAbility HandleInput(MenuInput input, PMPlayerCharacter character, PMBattle caller){
        if(input == MenuInput.Back){
            //parentGUI.
        }else if(input == MenuInput.Select){
            var ability = character.GetBasicAttack();
            ability.SetTargets(new PMCharacter[]{caller.PositionLookup(PMBattleUtilities.BattlePos.EnemyOne)});//TODO make conform with selection functions
            return ability;
        }
        return null;
    }
}
