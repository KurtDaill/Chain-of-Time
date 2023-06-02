using Godot;
using System;

public abstract partial class BattleMenu : Control
{
    public BattleGUI parentGUI;
    public enum MenuInput
    {
        Up,
        Right,
        Down,
        Left,
        Select,
        Back,
        None,
    }

    public override void _Ready(){
        parentGUI = (BattleGUI) GetParent();
    }

    //Run when this menu is opened, resets values as needed from previous uses
    public virtual void OnOpen(PlayerCombatant character, Battle caller){
        this.Visible = true;
    }

    //Handles input from the core Menu Command
    //Returns a new menu in the scenario we have to switch between menus
    public virtual PlayerAbility HandleInput(MenuInput input, PlayerCombatant character, Battle caller){ return null;}
}
