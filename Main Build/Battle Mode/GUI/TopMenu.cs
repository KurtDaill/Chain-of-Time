using Godot;
using System;

public class TopMenu : BattleMenu
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

    public override void OnOpen(){
        base.OnOpen();
        if(highlightedTab != -1)menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = false;
        highlightedTab = -1;
    }
        
    public override void _Process(float delta)
    {
       //Tab Animation
       if(highlightedTab != -1) menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = true;
    }

    public override void HandleInput(MenuInput input, out PMPlayerAbility ability)
    {
        ability = null;
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
                    case 1:
                        //Returns the Item Menu
                        parentGUI.ChangeMenu(2);
                        break;
                    case 2:
                        //Returns the Attack Menu
                        parentGUI.ChangeMenu(3);
                        break;
                    case 3:
                        //Returns the Skill Menu
                        parentGUI.ChangeMenu(4);
                        break;
                }
                break;
        }
    }
}
