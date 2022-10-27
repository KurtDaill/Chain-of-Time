using Godot;
using System;

public class SkillMenu : BattleMenu
{
    
    public AnimationPlayer menuAnim;
    public AudioStreamPlayer rejectSound;
    public SkillCard[] cards = new SkillCard[4];
    private int selectedOption = -1;
    private int availableCards = 0;
    public override void _Ready()
    {
        base._Ready();
        menuAnim = (AnimationPlayer) GetNode("AnimationPlayer");
        rejectSound = GetNode<AudioStreamPlayer>("SelectError");

        cards[0] = (SkillCard)GetNode("Card 1");
        cards[1] = (SkillCard)GetNode("Card 2");
        cards[2] = (SkillCard)GetNode("Card 3");
        cards[3] = (SkillCard)GetNode("Card 4");

        
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
    }
    public override void OnOpen(PMPlayerCharacter character, PMBattle caller)
    {
        var skills = character.GetAbilities();
        for(int i = 0; i < 4; i++){
            if(skills[i] != null){
                cards[i].SetDisplay(skills[i].GetAbilityName(), skills[i].GetRulesText(), skills[i].GetAbilityType(), skills[i].GetAbilityAlignment(), skills[i].GetSPCost());
                availableCards++;
            }else{
                cards[i].Visible= false;
            }
        }
        base.OnOpen(character, caller);
        menuAnim.Play("Enter");
        //Check current characters readied skills, setup cards to match that data
    }

    public override PMPlayerAbility HandleInput(MenuInput input, PMPlayerCharacter character, PMBattle caller){       
        var oldCard = selectedOption;
        if(selectedOption == -1){
            if(input != MenuInput.None){
                selectedOption = 0;
            }
        }else{
            switch(input){
                case MenuInput.Right : 
                    selectedOption++;
                    if(selectedOption >= availableCards){
                        selectedOption = availableCards-1;
                    }
                    break;
                case MenuInput.Left :
                    selectedOption--;
                    if(selectedOption < 0){
                        selectedOption = 0;
                    }
                    break;
                case MenuInput.Select : //TODO: Should go to a "Targeting" menu
                    if(character.ChargeSP(character.GetAbilities()[selectedOption].GetSPCost())){ //Returns false if player can't pay
                        menuAnim.Play("Exit");
                        TargetingMenu tMenu = (TargetingMenu) parentGUI.menus[5];
                        tMenu.SetAbilityForTargeting(character.GetAbilities()[selectedOption]);
                        parentGUI.ChangeMenu(5, character, caller);
                        return null;
                    }else{
                        rejectSound.Play();
                        return null;
                    }                 
                    
            }
        }
        if(oldCard != selectedOption){
            if(oldCard != -1) cards[oldCard].Stow();
            cards[selectedOption].Draw();
        }
        if(input == MenuInput.Back){
            menuAnim.Play("Exit");
            parentGUI.ChangeMenu(0, character, caller);
            return null;
        }
        return null;
    }
}
