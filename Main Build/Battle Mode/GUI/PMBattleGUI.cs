using Godot;
using System;
using static BattleMenu;
public class PMBattleGUI : Control //TODO Migrate a lot of this functionality to PlayerMenuSelection
{

    public BattleMenu currentMenu;
    public BattleMenu lastMenu;
    public PMBattle parentBattle;
    public int playerCharacterSelected = 0;
    public BattleMenu[] menus = new BattleMenu[5];
    public override void _Ready(){
        currentMenu = (BattleMenu) GetNode("Top Menu");
        parentBattle = (PMBattle) GetNode("../..");    
        menus[0] = (BattleMenu) GetNode("Top Menu");
        menus[2] = (BattleMenu) GetNode("Item Menu");
        menus[3] = (BattleMenu) GetNode("Attack Menu");
        menus[4] = (BattleMenu) GetNode("Skill Menu");
    }

    public void ResetGUIState(){
        currentMenu.Visible = false;
        lastMenu = currentMenu;
        currentMenu = (BattleMenu) GetNode("Top Menu");
        currentMenu.OnOpen();
    }

    public void SetForNewPlayerMenuSelection(){
        playerCharacterSelected = 0;
        ResetGUIState();
    }

    public void ChangeMenu(int newMenuIndex){
        currentMenu.Visible = false;
        lastMenu = currentMenu;
        currentMenu = menus[newMenuIndex];
        currentMenu.OnOpen();
    }
}
