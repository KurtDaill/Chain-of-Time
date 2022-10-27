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

    private Control playerCharacterReadouts;
    public BattleMenu[] menus = new BattleMenu[6];
    public override void _Ready(){
        currentMenu = (BattleMenu) GetNode("Top Menu");
        parentBattle = (PMBattle) GetNode("../..");    
        menus[0] = (BattleMenu) GetNode("Top Menu");
        menus[2] = (BattleMenu) GetNode("Item Menu");
        menus[3] = (BattleMenu) GetNode("Attack Menu");
        menus[4] = (BattleMenu) GetNode("Skill Menu");
        menus[5] = (BattleMenu) GetNode("Targeting Menu");
        playerCharacterReadouts = GetNode<Control>("Readouts");
    }

    //Returns the finished Queue when complete
    public Queue<PMPlayerAbility> Execute(MenuInput input, PMBattle caller){
        var temp = currentMenu.HandleInput(input, playersAbleToAct[abilitiesQueued.Count], caller);
        if(temp != null) abilitiesQueued.Enqueue(temp);
        if(abilitiesQueued.Count == playersAbleToAct.Length) return abilitiesQueued; //If all players have set an ability, we go to the next step in the battle
        return null;
    }
    public void ShowGUI(){
        currentMenu.Visible = true;
        playerCharacterReadouts.Visible = true;
    }

    public void HideGUI(bool keepReadouts = true){
        currentMenu.Visible = false;
        if(!keepReadouts) playerCharacterReadouts.Visible = false;
    }

    public void ResetGUIState(PMPlayerCharacter[] characters, PMBattle caller){
        abilitiesQueued = new Queue<PMPlayerAbility>();
        playersAbleToAct = characters;
        currentMenu.Visible = false;
        lastMenu = currentMenu;
        currentMenu = (BattleMenu) GetNode("Top Menu");
        currentMenu.OnOpen(playersAbleToAct[abilitiesQueued.Count], caller);
        ShowGUI();
    }

    //Changes menu. Pass -1 into newMenuIndex to goto the last menu
    public void ChangeMenu(int newMenuIndex, PMPlayerCharacter character, PMBattle caller){
        currentMenu.Visible = false;
        if(newMenuIndex == -1){
            var temp = currentMenu;
            currentMenu = lastMenu;
            lastMenu = temp;
        }else{
            lastMenu = currentMenu;
            currentMenu = menus[newMenuIndex];
        }
        currentMenu.OnOpen(character, caller);
    }
}
