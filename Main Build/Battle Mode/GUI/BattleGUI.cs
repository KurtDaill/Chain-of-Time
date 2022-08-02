using Godot;
using System;

public class BattleGUI : Control //TODO Migrate a lot of this functionality to PlayerMenuSelection
{

    public BattleMenu currentMenu;
    public BattleMenu lastMenu;

    public Battle parentBattle;

    public int playerCharacterSelected = 0;
    public PlayerMenuSelection parentCommand;
    public BattleMenu[] menus = new BattleMenu[5];
    public override void _Ready(){
        currentMenu = (BattleMenu) GetNode("Top Menu");
        parentBattle = (Battle) GetNode("../..");    
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

    //Called by menu's once they've gotten input from the player that indicates the next command needed.
    public void EnterCommand(BattleCommand next){
        parentBattle.AddCommand(next);
        NextCharacter();
        //parentBattle.NextCommand();
        
    }

    public void EnterCommand(BattleCommand[] nextArr){
        for(int i = 0; i < nextArr.Length; i++){
            parentBattle.AddCommand(nextArr[i]);
        }
        NextCharacter();
        //parentBattle.NextCommand();
    }

    private void NextCharacter(){
        if(playerCharacterSelected < 2){
            if(parentBattle.activeCombatants[playerCharacterSelected + 1] != null){
                parentBattle.activeCombatants[playerCharacterSelected].DeselectMe();
                playerCharacterSelected ++;
                parentBattle.activeCombatants[playerCharacterSelected].SelectMe();
                ResetGUIState();
                return;
            }
        }
        parentBattle.activeCombatants[playerCharacterSelected].DeselectMe();
        parentBattle.AddCommand(new BattleCommandEnemyAttacks());
        parentBattle.NextCommand();
    }
}
