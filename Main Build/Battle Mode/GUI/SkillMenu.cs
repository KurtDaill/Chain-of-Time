using Godot;
using System;

public class SkillMenu : BattleMenu
{
    /*
    public AnimationPlayer menuAnim;
    public SkillCard[] cards = new SkillCard[4];
    private int selectedOption = -1;
    private int availableCards = 0;
    private PlayerCombatant activeCharacter;

    public override void _Ready()
    {
        base._Ready();
        menuAnim = (AnimationPlayer) GetNode("AnimationPlayer");
        

        cards[0] = (SkillCard)GetNode("Card 1");
        cards[1] = (SkillCard)GetNode("Card 2");
        cards[2] = (SkillCard)GetNode("Card 3");
        cards[3] = (SkillCard)GetNode("Card 4");

        
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
    }
    public override void OnOpen()
    {
        activeCharacter = (PlayerCombatant) parentGUI.parentBattle.activeCombatants[parentGUI.playerCharacterSelected]; //TODO Reconfigure when player selection is implemented
        for(int i = 0; i < 4; i++){
            var skill = activeCharacter.GetSkill(i);
            if(skill != null){
                cards[i].SetDisplay(skill.GetAbilityName(), skill.GetRulesText(), skill.GetAbilityType(), skill.GetCost());
                availableCards++;
            }else{
                cards[i].Visible= false;
            }
        }
        base.OnOpen();
        menuAnim.Play("Enter");
        //Check current characters readied skills, setup cards to match that data
    }

    public override BattleMenu HandleInput(MenuInput input){       
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
                case MenuInput.Select :
                        //Select the thing!
                    break;
            }
        }
        if(oldCard != selectedOption){
            if(oldCard != -1) cards[oldCard].Stow();
            cards[selectedOption].Draw();
        }
        if(input == MenuInput.Back){
            menuAnim.Play("Exit");
            return parentGUI.lastMenu;
        }
        return null;
    }
    */
}
