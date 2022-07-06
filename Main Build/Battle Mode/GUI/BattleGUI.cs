using Godot;
using System;

public class BattleGUI : Control
{

    public BattleMenu currentMenu;
    public BattleMenu lastMenu;

    public Battle parentBattle;
    public PlayerMenuSelection parentCommand;
    public BattleMenu[] menus = new BattleMenu[5];
    public override void _Ready(){
        currentMenu = (BattleMenu) GetNode("Top Menu");
        parentBattle = (Battle) GetNode("../..");    
        menus[0] = (BattleMenu) GetNode("Top Menu");
        menus[3] = (BattleMenu) GetNode("Attack Menu");
    }

    public void ResetGUIState(){
        currentMenu = (BattleMenu) GetNode("Top Menu");
    }

    //Called by menu's once they've gotten input from the player that indicates the next command needed.
    public void EnterCommand(BattleCommand next){
        parentBattle.AddCommand(next);
        parentBattle.NextCommand();
    }

    public void EnterCommand(BattleCommand[] nextArr){
        for(int i = 0; i < nextArr.Length; i++){
            parentBattle.AddCommand(nextArr[i]);
        }
        parentBattle.NextCommand();
    }
}
