using Godot;
using System;

public partial class AttackMenu : BattleMenu
{
    public override void OnOpen(PlayerCombatant character, Battle caller)
    {
        base.OnOpen(character, caller);
        this.GetNode<RichTextLabel>("Backboard/Rules Text").Text = character.GetBasicAttack().GetRulesText();
    }

    public override PlayerAbility HandleInput(MenuInput input, PlayerCombatant character, Battle caller){
        if(input == MenuInput.Back){
            parentGUI.ChangeMenu(0, character);
            return null;
        }else if(input == MenuInput.Select){
            //var ability = character.GetBasicAttack();
            //ability.SetTargets(new PMCharacter[]{caller.PositionLookup(PMBattleUtilities.BattlePos.EnemyOne)});//TODO make conform with selection functions
            //return ability;
            TargetingMenu tMenu = (TargetingMenu) parentGUI.menus[5];
            tMenu.SetAbilityForTargeting(character.GetBasicAttack());
            parentGUI.ChangeMenu(5, character);
            return null;
        }
        return null;
    }
}
