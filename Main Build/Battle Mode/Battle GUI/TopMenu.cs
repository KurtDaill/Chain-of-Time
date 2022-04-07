using Godot;
using System;

public class TopMenu : BattleMenu
{
    //TODO Highlight Animations
    //TODO Highlight Selector

    [Export]
    TextureRect[] menuTabs = new TextureRect[5];

    int highlightedTab = -1;

    public override void _Ready()
    {
        base._Ready();   
        menuTabs[0] = (TextureRect) GetNode("PartyButton");
        menuTabs[1] = (TextureRect) GetNode("ItemButton");
        menuTabs[2] = (TextureRect) GetNode("AtkButton");
        menuTabs[3] = (TextureRect) GetNode("SkillButton");
    }

    public override void OnOpen(){
        base.OnOpen();
        menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = false;
        highlightedTab = -1;
    }
        
    public override void _Process(float delta)
    {
       //Tab Animation
       if(highlightedTab != -1) menuTabs[highlightedTab].GetNode<TextureRect>("Highlight").Visible = true;
    }

    public override BattleMenu HandleInput(MenuInput input)
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
                    case 2:
                        //Returns the Attack Menu
                        return parentGUI.menus[3];
                }
                break;
        }
        return null;
    }
}
