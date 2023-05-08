using Godot;
using System;

public partial class ItemMenu : BattleMenu
{
    
    /*
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    private (Item, int)[] playerInventory;
    private AnimationPlayer animPlayer;

    public static int numberOfTabs = 5;
    private Label[] namePlates = new Label[numberOfTabs];
    private Label[] counts = new Label[numberOfTabs];

    private int currentFocusItem = 0;
    public override void _Ready()
    {
        animPlayer = (AnimationPlayer) GetNode("AnimationPlayer");
        for(int i = 0; i < numberOfTabs; i++){
            namePlates[i] = (Label) GetNode("Item TabBar/Item Tab " + i + "/Name");
            counts[i] = (Label) GetNode("Item TabBar/Item Tab " + i + "/Count");
        }
        base._Ready();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(double delta)
//  {
//      
//  }
    public override void OnOpen(){
        var list = PartyData.Instance().GetBattleItems();
        list.Add((null, 0));
        list.Add((null, 0));
        list.Add((null, 0));
        playerInventory = list.ToArray();
        
        animPlayer.Play("Ready to Scroll Down"); //Sets the item tabs in the correct positions
        animPlayer.Advance(0.1F);
        SetLabels(0);
        base.OnOpen(); 
    }
*/
    public override PMPlayerAbility HandleInput(MenuInput input, PMPlayerCharacter character, PMBattle caller){
        if(input == MenuInput.Back){
            parentGUI.ChangeMenu(0, character, caller);
            return null;
        }
        return null;
    }
/*
    public void ScrollDown(){
        /*
            Set Item TabBar such that "Item Tab 0"  is the top invisible tab, and "Item Tab 4" is visibly on the bottom
            Set Labels correctly
            Play Scroll Up
        */
/*        if(currentFocusItem == (playerInventory.Length - 4)) return;
        animPlayer.Play("Ready to Scroll Down");
        animPlayer.Advance(0.1F); 
        SetLabels(0);
        animPlayer.Play("Scroll Down");
        currentFocusItem ++;
    }

    public void ScrollUp(){
        /*
            Set Item TabBar such that "Item Tab 0"  is the top visible tab, and "Item Tab 4" is invisibly on the bottom
            Set Labels correctly
            Play Scroll Up
        */
/*        if(currentFocusItem == 0) return;
        animPlayer.Play("Ready to Scroll Up");
        animPlayer.Advance(0.1F); 
        SetLabels(1);
        animPlayer.Play("Scroll Up");
        currentFocusItem --;
    }

    private void SetLabels(int focusTab){
        /*
            This function sets the text of the tabs to be appropriate, as our ui animations can only scroll 1 tab up or down, we manipulate the text between those scrolls to give the
            impression of scrolling smoothly through an ordered list.
            focusTab is meant to indicates which itemTab should contain the item the player focusing on is, and therefore where the rest of the list falls.
            the for loop populates that list correctly.
        *//*
        if(focusTab != 0 && focusTab != 1) throw new IndexOutOfRangeException();
        for(int i = 0; i < numberOfTabs; i++){
            if(playerInventory[currentFocusItem + (i - focusTab)].Item1 == null){
                namePlates[i].Text = "";
                counts[i].Text = "";
                continue;
            }
            namePlates[i].Text = playerInventory[currentFocusItem + (i - focusTab)].Item1.GetName();
            counts[i].Text = "x" + playerInventory[currentFocusItem + (i - focusTab)].Item2;
        }
    }
    */
}
