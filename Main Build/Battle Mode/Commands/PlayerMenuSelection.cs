using Godot;
using System;

public class PlayerMenuSelection : BattleCommand
{
    BattleGUI gui;
    BattleMenu temp;
    BattleMenu.MenuInput input;

    public override void Enter(Battle parent){
        this.parent = parent;
        gui = (BattleGUI) parent.GetNode(parent.GUI);
    }
    public override void Execute(float delta, Battle parent){
        if(Input.IsActionJustPressed("ui_up")){
            input = BattleMenu.MenuInput.Up;
        }else if(Input.IsActionJustPressed("ui_right")){
            input = BattleMenu.MenuInput.Right;
        }else if(Input.IsActionJustPressed("ui_down")){
            input = BattleMenu.MenuInput.Down;
        }else if(Input.IsActionJustPressed("ui_left")){
            input = BattleMenu.MenuInput.Left;
        }else if(Input.IsActionJustPressed("ui_back")){
            input = BattleMenu.MenuInput.Back;
        }else if(Input.IsActionJustPressed("ui_accept")){
            input = BattleMenu.MenuInput.Select;
        }else{
            return;
        }

        temp = gui.currentMenu.HandleInput(input);

        if(temp != null){
            gui.currentMenu.Visible = false;
            gui.lastMenu = gui.currentMenu;
            gui.currentMenu = temp;
            gui.currentMenu.OnOpen();
        }
    }

    //Called by menu's once they've gotten input from the player that indicates the next command needed.
    public void EnterCommand(BattleCommand next){
        parent.AddCommand(next);
    }

    public override void Undo()
    {
        throw new NotImplementedException();
    }

    public override void Exit(){
        gui.currentMenu.Visible = false;
    }
}
