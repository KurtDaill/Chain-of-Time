using Godot;
using System;

public partial class AttackMenu : BattleMenu
{
    private AudioStreamPlayer selectError;
    public override void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI)
    {
        base.OnOpen(character, caller, parentGUI);
        this.GetNode<Label>("Backboard/Attack Name").Text = character.GetBasicAttack().GetName();
        this.GetNode<RichTextLabel>("Backboard/Rules Text").Text = character.GetBasicAttack().GetRulesText();
        selectError = GetNode<AudioStreamPlayer>("SelectError");
    }

    public override PlayerAbility HandleInput(MenuInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        if(input == MenuInput.Back){
            parentGUI.ChangeMenu(0, character);
            return null;
        }else if(input == MenuInput.Select){
            //var ability = character.GetBasicAttack();
            //ability.SetTargets(new PMCharacter[]{caller.PositionLookup(PMBattleUtilities.BattlePos.EnemyOne)});//TODO make conform with selection functions
            //return ability;
            if(!character.GetBasicAttack().GetEnabledPositions().Contains(caller.GetRoster().GetCharacterVirtualPosition(character).Item2)){
                selectError.Play();
                return null;
            }
            TargetingMenu tMenu = (TargetingMenu) parentGUI.menus[5];
            tMenu.SetAbilityForTargeting(character.GetBasicAttack(), parentGUI);
            parentGUI.ChangeMenu(5, character);
            return null;
        }
        return null;
    }
}
