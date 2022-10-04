using Godot;
using System;

public class AttackMenu : BattleMenu
{
    public override void OnOpen()
    {
        base.OnOpen();
    }

    public override void HandleInput(MenuInput input, out PMPlayerAbility ability){  
        ability = null;
        if(input == MenuInput.Back){
            //parentGUI.
        }else if(input == MenuInput.Select){
            ability = parentGUI.parentBattle.GetPlayerCharacter(parentGUI.playerCharacterSelected).GetBasicAttack();
        }
        return;
    }
}
