using Godot;
using System;

public class TopMenu : Control
{
    //TODO Highlight Animations
    //TODO Highlight Selector

    //[Export]
    TextureRect[] menuTabs = new TextureRect[4];
    TextureRect[] menuTabHighlights = new TextureRect[4];

    public override void _Ready()
    {
        Godot.Collections.Array children = GetChildren();
        foreach(Node node in children){
            switch(node.Name){
                case "PartyButton" :
                    menuTabs[0] = (TextureRect) node;
                    break;
                case "ItemButton" :
                    menuTabs[1] = (TextureRect) node;
                    break; 
                case "AtkButton" :
                    menuTabs[2] = (TextureRect) node;
                    break;
                case "SkillButton" :
                    menuTabs[3] = (TextureRect) node;
                    break;   
            }
        }
    }
        
    public override void _Process(float delta)
    {
        //Highlight Animations
    }

    public void ChangeSeleciton(int select){

    }
}
