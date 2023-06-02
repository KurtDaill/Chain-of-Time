using Godot;
using System;
public partial class TopMenu : BattleMenu
{
    //TODO Highlight Animations

    [Export]
    TextureRect[] menuTabs = new TextureRect[5];

    int highlightedTab = -1;

    public override void _Ready()
    {
        base._Ready();   
        menuTabs[0] = (TextureRect) GetNode("./Button Hub/PartyButton");
        menuTabs[1] = (TextureRect) GetNode("./Button Hub/ItemButton");
        menuTabs[2] = (TextureRect) GetNode("./Button Hub/AtkButton");
        menuTabs[3] = (TextureRect) GetNode("./Button Hub/SkillButton");
    }

    public override void OnOpen(PlayerCombatant character, Battle caller){
        base.OnOpen(character, caller);
        if(highlightedTab != -1)menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = false;
        highlightedTab = -1;
    }
        
    public override void _Process(double delta)
    {
       //Tab Animation
       if(highlightedTab != -1) menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = true;
    }

    public override PlayerAbility HandleInput(MenuInput input, PlayerCombatant character, Battle caller)
    {
        switch(input){
            case MenuInput.Up: 
                if(highlightedTab != -1) menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = false;
                highlightedTab = 0;
                break;
            case MenuInput.Right: 
                if(highlightedTab != -1) menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = false;
                highlightedTab = 1; 
                break;
            case MenuInput.Down: 
                if(highlightedTab != -1) menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = false;
                highlightedTab = 2; 
                break;
            case MenuInput.Left: 
                if(highlightedTab != -1) menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = false;
                highlightedTab = 3; 
                break;
            case MenuInput.Select:
                switch(highlightedTab){
                    case 0:
                        //Goes to the Party Menu
                        //parentGUI.ChangeMenu(1,character, caller);
                        break;
                    case 1:
                        //Goes to the Item Menu
                        //parentGUI.ChangeMenu(2, character, caller);
                        break;
                    case 2:
                        //Goes to the Attack Menu
                        parentGUI.ChangeMenu(3, character);
                        break;
                    case 3:
                        //Goes to the Skill Menu
                        //parentGUI.ChangeMenu(4, character, caller);
                        break;
                }
                break;
            case MenuInput.Back:
                parentGUI.GotoPreviousCharacter();
                break;
        }
        return null;
    }
}
