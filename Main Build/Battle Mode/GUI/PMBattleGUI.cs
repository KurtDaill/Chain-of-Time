using Godot;
using System;
using System.Collections.Generic;
using static BattleMenu;
public class PMBattleGUI : Control //TODO Migrate a lot of this functionality to PlayerMenuSelection
{

    public BattleMenu currentMenu;
    public BattleMenu lastMenu;
    public PMBattle parentBattle;

    private PMPlayerCharacter[] playersAbleToAct;
    private Queue<PMPlayerAbility> abilitiesQueued;
    public BattleMenu[] menus = new BattleMenu[5];
    public override void _Ready(){
        currentMenu = (BattleMenu) GetNode("Top Menu");
        parentBattle = (PMBattle) GetNode("../..");    
        menus[0] = (BattleMenu) GetNode("Top Menu");
        menus[2] = (BattleMenu) GetNode("Item Menu");
        menus[3] = (BattleMenu) GetNode("Attack Menu");
        menus[4] = (BattleMenu) GetNode("Skill Menu");
    }

    //Returns the finished Queue when complete
    public Queue<PMPlayerAbility> Execute(MenuInput input, PMBattle caller){
        var temp = currentMenu.HandleInput(input, playersAbleToAct[abilitiesQueued.Count], caller);
        if(temp != null) abilitiesQueued.Enqueue(temp);
        if(abilitiesQueued.Count == playersAbleToAct.Length) return abilitiesQueued; //If all players have set an ability, we keep going
        return null;
    }
    public void ShowGUI(){
        Visible = true;
    }

    public void HideGUI(){
        Visible = false;
    }

    public void ResetGUIState(PMPlayerCharacter[] characters){
        abilitiesQueued = new Queue<PMPlayerAbility>();
        playersAbleToAct = characters;
        currentMenu.Visible = false;
        lastMenu = currentMenu;
        currentMenu = (BattleMenu) GetNode("Top Menu");
        currentMenu.OnOpen();
        ShowGUI();
    }

    public void ChangeMenu(int newMenuIndex){
        currentMenu.Visible = false;
        lastMenu = currentMenu;
        currentMenu = menus[newMenuIndex];
        currentMenu.OnOpen();
    }
}
