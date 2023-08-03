using Godot;
using System;

public partial class AttackMenu : BattleMenu
{
    private AudioStreamPlayer selectError;
    [Export(PropertyHint.File)]
    string catoCardTexture;
    [Export(PropertyHint.File)]
    string silverCardTexture;
    [Export(PropertyHint.File)]
    string lucieneCardTexture;
    [Export(PropertyHint.File)]
    string catoCardTheme;
    [Export(PropertyHint.File)]
    string lucieneCardTheme;
    [Export(PropertyHint.File)]
    string silverCardTheme;
    public override void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI)
    {
        base.OnOpen(character, caller, parentGUI);
        this.GetNode<Label>("Backboard/Attack Name").Text = character.GetBasicAttack().GetName();
        this.GetNode<RichTextLabel>("Backboard/Rules Text").Text = character.GetBasicAttack().GetRulesText();
        Texture2D attackBackboardTexture = null;
        Theme attackMenuTheme = null;
        switch(character.Name){
            case "Cato" :
                attackBackboardTexture = GD.Load<Texture2D>(catoCardTexture); 
                attackMenuTheme = GD.Load<Theme>(catoCardTheme);
                break;
            case "Silver" : 
                attackBackboardTexture = GD.Load<Texture2D>(silverCardTexture);
                attackMenuTheme = GD.Load<Theme>(silverCardTheme); 
                break;
            case "Luciene" :
                attackBackboardTexture = GD.Load<Texture2D>(lucieneCardTexture); 
                attackMenuTheme = GD.Load<Theme>(lucieneCardTheme); 
                break;
        }
        this.GetNode<TextureRect>("Backboard").Texture = attackBackboardTexture;
        this.GetNode<TextureRect>("Backboard").Theme = attackMenuTheme;
        selectError = GetNode<AudioStreamPlayer>("SelectError");
        parentGUI.menus[5].OnOpen(character, caller, parentGUI);
        ((NewTargetingMenu)parentGUI.menus[5]).SetAbilityForTargeting(character.GetBasicAttack(), character, caller, parentGUI);
    }

    public override PlayerAbility HandleInput(MenuInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        if(input == MenuInput.Back){
            parentGUI.ChangeMenu(0, character);
            ((NewTargetingMenu)parentGUI.menus[5]).SetPointers(caller);
            return null;
        }else{  
            return parentGUI.menus[5].HandleInput(input, character, caller, parentGUI);
        }
        /*else if(input == MenuInput.Select){
            //var ability = character.GetBasicAttack();
            //ability.SetTargets(new PMCharacter[]{caller.PositionLookup(PMBattleUtilities.BattlePos.EnemyOne)});//TODO make conform with selection functions
            //return ability;
            if(!character.GetBasicAttack().GetenabledRanks().Contains(caller.GetRoster().GetCharacterVirtualPosition(character).GetRank())){
                selectError.Play();
                return null;
            }
            NewTargetingMenu tMenu = (NewTargetingMenu) parentGUI.menus[5];
            tMenu.SetAbilityForTargeting(character.GetBasicAttack(), character, caller, parentGUI);
            parentGUI.ChangeMenu(5, character);
            return null;
        }
        */
    }
}
