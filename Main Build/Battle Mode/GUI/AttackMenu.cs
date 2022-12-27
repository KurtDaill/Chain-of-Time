using Godot;
using System;

public partial class AttackMenu : BattleMenu
{
    public override void OnOpen(PMPlayerCharacter character, PMBattle caller)
    {
        base.OnOpen(character, caller);
        this.GetNode<RichTextLabel>("Backboard/Rules Text").Text = (character.GetBasicAttack().GetRulesText());
    }

    public override PMPlayerAbility HandleInput(MenuInput input, PMPlayerCharacter character, PMBattle caller){
        if(input == MenuInput.Back){
            parentGUI.ChangeMenu(0, character, caller);
            return null;
        }else if(input == MenuInput.Select){
            //var ability = character.GetBasicAttack();
            //ability.SetTargets(new PMCharacter[]{caller.PositionLookup(PMBattleUtilities.BattlePos.EnemyOne)});//TODO make conform with selection functions
            //return ability;
            TargetingMenu tMenu = (TargetingMenu) parentGUI.menus[5];
            tMenu.SetAbilityForTargeting(character.GetBasicAttack());
            parentGUI.ChangeMenu(5, character, caller);
            return null;
        }
        return null;
    }
}
