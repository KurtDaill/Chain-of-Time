using Godot;
using System;

public class BattleGUI : Control
{
    [Export]
    public TopMenu topMenu;
    [Export]
    public Control attackMenu;
    [Export]
    public Control itemMenu;
    [Export]
    public Control skillMenu;
    [Export]
    public Control partyMenu;

    public Control[] menus = new Control[5];

    public int currentMenu = 0;

    public override void _Ready(){
        menus[0] = topMenu;
        menus[1] = partyMenu;
        menus[2] = itemMenu;
        menus[3] = attackMenu;
        menus[4] = skillMenu;
    }

    public void switchMenu(int newMenu){
        menus[currentMenu].Visible = false;
        currentMenu = newMenu;
        menus[currentMenu].Visible = true;
    }
     
}
