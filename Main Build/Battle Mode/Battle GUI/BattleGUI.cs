using Godot;
using System;

public class BattleGUI : Control
{

    public BattleMenu currentMenu;
    public BattleMenu lastMenu;

    public Battle parentBattle;
    public BattleMenu[] menus = new BattleMenu[5];
    public override void _Ready(){
        currentMenu = (BattleMenu) GetNode("Top Menu");
        parentBattle = (Battle) GetNode("../..");    
        menus[0] = (BattleMenu) GetNode("Top Menu");
        menus[3] = (BattleMenu) GetNode("Attack Menu");
    }

    //Called by menu's once they've gotten input from the player that indicates the next command needed.
    public void EnterCommand(BattleCommand next){
        parentBattle.AddCommand(next);
        parentBattle.NextCommand();
    }

    public void EnterCommand(BattleCommand[] nextArr){
        foreach(BattleCommand com in nextArr){
            parentBattle.AddCommand(com);
        }
        parentBattle.NextCommand();
    }
}
