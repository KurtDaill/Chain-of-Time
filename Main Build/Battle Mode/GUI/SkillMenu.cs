using Godot;
using System;
using static BattleUtilities;

public partial class SkillMenu : BattleMenu
{
    public AnimationPlayer menuAnim;
    public AudioStreamPlayer rejectSound;
    public SkillCard[] cards = new SkillCard[4];
    private int selectedOption = -1;
    private int availableCards = 0;

    bool noSkills;
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

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
    public override void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI)
    {
        selectedOption = 0;
        availableCards = 0;
        noSkills = false;
        var skills = character.GetSkills();
        if(skills.Length == 0){ //Handles the special case if there are no skills
                cards[0].SetDisplay("No Skills", "", "", AbilityAlignment.Normal, 0);
                availableCards++;
                noSkills = true;
        }
        else{
            for(int i = 0; i < skills.Length; i++){
                if(skills[i] != null){
                    cards[i].SetDisplay(skills[i].GetName(), skills[i].GetRulesText(), skills[i].GetSkilType(), skills[i].GetAbilityAlignment(), skills[i].GetSPCost());
                    availableCards++;
                }else{
                    cards[i].Visible= false;
                }
            }
        }
        base.OnOpen(character, caller, parentGUI);
        menuAnim.Play("Enter");
        //Check current characters readied skills, setup cards to match that data
    }

    public override Ability HandleInput(MenuInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){       
        var oldCard = selectedOption;
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
                if(noSkills == false && character.ChargeSP(character.GetSkills()[selectedOption].GetSPCost())){ //Returns false if player can't pay
                    menuAnim.Play("Exit");
                    TargetingMenu tMenu = (TargetingMenu) parentGUI.menus[5];
                    tMenu.SetAbilityForTargeting(character.GetSkills()[selectedOption]);
                    parentGUI.ChangeMenu(5, character);
                    return null;
                }else{
                    rejectSound.Play();
                    return null;
                }                 
                
        }
        if(oldCard != selectedOption){
            if(oldCard != -1) cards[oldCard].Stow();
            cards[selectedOption].Draw();
        }
        if(input == MenuInput.Back){
            menuAnim.Play("Exit");
            parentGUI.ChangeMenu(0, character);
            return null;
        }
        return null;
    }
}
