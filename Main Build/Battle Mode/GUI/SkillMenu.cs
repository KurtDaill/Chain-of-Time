using Godot;
using System;
using static BattleUtilities;

public partial class SkillMenu : BattleMenu
{
    public AnimationPlayer menuAnim;
    public AudioStreamPlayer rejectSound;
    public SkillCard[] cards = new SkillCard[3];
    private int selectedOption = -1;
    private int availableCards = 0;
    
    bool noSkills;
    public override void _Ready()
    {
        base._Ready();
        menuAnim = (AnimationPlayer) GetNode("AnimationPlayer");
        rejectSound = GetNode<AudioStreamPlayer>("SelectError");

        cards[0] = (SkillCard)GetNode("Card Rack/Card 1");
        cards[1] = (SkillCard)GetNode("Card Rack/Card 2");
        cards[2] = (SkillCard)GetNode("Card Rack/Card 3");
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
                cards[0].SetDisplay("No Skills", "", "", AbilityAlignment.Normal, 0, new Godot.Collections.Array<BattleRank>());
                availableCards++;
                noSkills = true;
        }
        else{
            for(int i = 0; i < 3; i++){
                if(i < skills.Length){
                    cards[i].SetDisplay(skills[i].GetName(), skills[i].GetRulesText(), skills[i].GetSkilType(), skills[i].GetAbilityAlignment(), skills[i].GetSPCost(), skills[i].GetenabledRanks());
                    availableCards++;
                    if(i == 0) cards[i].Draw();
                    else cards[i].Stow();
                }else{
                    cards[i].Visible = false;
                }
            }
        }
        base.OnOpen(character, caller, parentGUI);
        //menuAnim.Play("Enter");
        //Check current characters readied skills, setup cards to match that data
    }

    public override PlayerAbility HandleInput(PlayerInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){       
        var oldCard = selectedOption;
        switch(input){
            case PlayerInput.Right : 
                selectedOption++;
                if(selectedOption >= availableCards){
                    selectedOption = availableCards-1;
                }
                break;
            case PlayerInput.Left :
                selectedOption--;
                if(selectedOption < 0){
                    selectedOption = 0;
                }
                break;
            case PlayerInput.Select : //TODO: Should go to a "Targeting" menu --- ChargeSP returns false if player can't pay, and MUST BE AT THE END OF THIS CONDITIONAL!!!
                if(noSkills == false && character.GetSkills()[selectedOption].GetenabledRanks().Contains(caller.GetRoster().GetCharacterVirtualPosition(character).GetRank()) && character.ChargeSP(character.GetSkills()[selectedOption].GetSPCost())){ 
                    //menuAnim.Play("Exit");
                    NewTargetingMenu tMenu = (NewTargetingMenu) parentGUI.menus[5];
                    tMenu.SetAbilityForTargeting(character.GetSkills()[selectedOption], character, caller, parentGUI);
                    parentGUI.ChangeMenu(5, character);
                    return null;
                }else{
                    if(!character.GetSkills()[selectedOption].GetenabledRanks().Contains(character.GetPosition().GetRank())){
                        cards[selectedOption].FlashFriendlyPips();
                    }
                    rejectSound.Play();
                    return null;
                }                 
                
        }
        if(oldCard != selectedOption){
            if(oldCard != -1) cards[oldCard].Stow();
            cards[selectedOption].Draw();
        }
        if(input == PlayerInput.Back){
            //menuAnim.Play("Exit");
            parentGUI.ChangeMenu(0, character);
            return null;
        }
        return null;
    }
}
