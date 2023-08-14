using Godot;
using System;
using static GameplayUtilities;

public abstract partial class BattleMenu : Control
{
    //public BattleGUI parentGUI;
    

    public override void _Ready(){
        //parentGUI = (BattleGUI) GetParent();
    }

    //Run when this menu is opened, resets values as needed from previous uses
    public virtual void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        this.Visible = true;
    }

    //Handles input from the core Menu Command
    //Returns a new menu in the scenario we have to switch between menus
    public virtual PlayerAbility HandleInput(PlayerInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){ return null;}
}
