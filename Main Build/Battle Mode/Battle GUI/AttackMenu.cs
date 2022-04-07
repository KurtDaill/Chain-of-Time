using Godot;
using System;

public class AttackMenu : BattleMenu
{
    public override void OnOpen()
    {
        base.OnOpen();
    }

    public override BattleMenu HandleInput(MenuInput input){
        if(input == MenuInput.Back){
            return parentGUI.lastMenu;
        }else if(input == MenuInput.Select){
            parentGUI.EnterCommand(new EnemyAttacks());
        }
        return null;
    }
}
