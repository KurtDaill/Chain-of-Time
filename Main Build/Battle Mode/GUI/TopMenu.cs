using Godot;
using System;
public partial class TopMenu : BattleMenu
{
    //TODO Highlight Animations

    [Export]
    TextureRect[] menuTabs = new TextureRect[4];

    int highlightedTab = 0;

    ReadoutContainer parent;

    public override void _Ready()
    {
        base._Ready();   
        menuTabs[0] = (TextureRect) GetNode("Attack Button");
        menuTabs[1] = (TextureRect) GetNode("Skill Button");
        menuTabs[2] = (TextureRect) GetNode("Party Button");
        menuTabs[3] = (TextureRect) GetNode("Item Button");
        parent = GetParent<ReadoutContainer>();
    }

    public override void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        base.OnOpen(character, caller, parentGUI);
        menuTabs[highlightedTab].GetNode<TextureRect>("Pointer").Visible = false;
        highlightedTab = 0;
        menuTabs[highlightedTab].GetNode<TextureRect>("Pointer").Visible = true;
    }
        
    public override void _Process(double delta)
    {
    }

    public override Ability HandleInput(MenuInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI)
    {
        switch(input){
            case MenuInput.Up: 
            menuTabs[highlightedTab].GetNode<TextureRect>("Pointer").Visible = false;
                if(highlightedTab > 0) highlightedTab --;
                else highlightedTab = 3;
                menuTabs[highlightedTab].GetNode<TextureRect>("Pointer").Visible = true; 
                break;
            case MenuInput.Down: 
                menuTabs[highlightedTab].GetNode<TextureRect>("Pointer").Visible = false;
                if(highlightedTab < 3) highlightedTab ++;
                else highlightedTab = 0;
                menuTabs[highlightedTab].GetNode<TextureRect>("Pointer").Visible = true; 
                break;
            case MenuInput.Select:
                switch(highlightedTab){
                    case 0:
                        //Goes to the Attack Menu
                        parentGUI.ChangeMenu(1,character);
                        break;
                    case 1:
                        //Goes to the Skill Menu
                        parentGUI.ChangeMenu(2, character);
                        break;
                    case 2:
                        //Goes to the Party Menu
                        parentGUI.ChangeMenu(3, character);
                        break;
                    case 3:
                        //Goes to the Item Menu
                        //parentGUI.ChangeMenu(4, character);
                        break;
                }
                break;
            case MenuInput.Back:
                parentGUI.GotoPreviousCharacter();
                break;
        }
        return null;
    }
}
