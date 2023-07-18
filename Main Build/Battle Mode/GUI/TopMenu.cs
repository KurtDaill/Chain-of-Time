using Godot;
using System;
public partial class TopMenu : BattleMenu
{
    //TODO Highlight Animations
    TextureRect[] menuTabs = new TextureRect[5];

    int highlightedTab = -1;

    public override void _Ready()
    {
        base._Ready();   
        menuTabs[0] = (TextureRect) GetNode("AttackButton");
        menuTabs[1] = (TextureRect) GetNode("SkillButton");
        menuTabs[2] = (TextureRect) GetNode("ItemButton");
        menuTabs[3] = (TextureRect) GetNode("PartyButton");
    }

    public override void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        base.OnOpen(character, caller, parentGUI);
        if(highlightedTab != -1)menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = false;
        highlightedTab = -1;
    }
        
    public override void _Process(double delta)
    {
       //Tab Animation
       if(highlightedTab != -1) menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = true;
    }

    public override PlayerAbility HandleInput(MenuInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI)
    {
        if(highlightedTab == -1){
            if(input != MenuInput.None){
                highlightedTab = 0;
                menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = true;
                menuTabs[highlightedTab].SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
            }
        }else{
            switch(input){
                case MenuInput.Up: 
                    if(highlightedTab > 0){
                        menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = false;
                        menuTabs[highlightedTab].SizeFlagsHorizontal = SizeFlags.ShrinkEnd;
                        highlightedTab--;
                    }
                    break;
                case MenuInput.Down: 
                    if(highlightedTab < 3){
                        menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = false;
                        menuTabs[highlightedTab].SizeFlagsHorizontal = SizeFlags.ShrinkEnd;
                        highlightedTab++;
                    }
                    break; 
                case MenuInput.Select:
                    switch(highlightedTab){
                        case 0:
                            //Goes to the Attack Menu
                            parentGUI.ChangeMenu(1,character);
                            break;
                        case 1:
                            //Goes to the Skill Menu
                            parentGUI.ChangeMenu(2, character);
                            break;
                        case 2:
                            //Goes to the Item Menu
                            //parentGUI.ChangeMenu(3, character);
                            break;
                        case 3:
                            //Goes to the Party Menu
                            parentGUI.ChangeMenu(4, character);
                            break;
                    }
                    break;
                case MenuInput.Back:
                    parentGUI.GotoPreviousCharacter();
                    break;
            } 
            menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = true;
            menuTabs[highlightedTab].SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
        }
        return null;
    }
}
