using Godot;
using System;

public abstract class BattleMenu : Control
{
    public BattleGUI parentGUI;
    public enum MenuInput
    {
        Up,
        Right,
        Down,
        Left,
        Select,
        Back
    }

    public override void _Ready(){
        parentGUI = (BattleGUI) GetParent();
    }

    //Run when this menu is opened, resets values as needed from previous uses
    public virtual void OnOpen(){
        this.Visible = true;
    }

    //Handles input from the core Menu Command
    //Returns a new menu in the scenario we have to switch between menus
    public virtual BattleMenu HandleInput(MenuInput input){return null;}
}
