using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using static GameplayUtilities;
public partial class ShopInterface : GameplayMode{
    public ShopButton[][] shopButton;
    protected int selectButtonX, selectButtonY;
    [Export]
    protected int defaultButtonX = 0;
    [Export]
    protected int defaultButtonY = 0;
    [Export]
    protected Camera3D shopCam;
    [Export]
    protected Label3D descritpionTextBox;
    [Export]
    protected Label3D descriptionTitleBox;

    protected GameplayMode returnMode = null;
    protected GameplayMode backOutMode;
    [Export]
    protected GameplayMode DebugBackOutMode = null;
    [Export]
    protected string defaultBoxTitle;
    [Export]
    protected string defaultBoxText;
    protected GameMaster gm;

    public override void _Ready()
    {
        base._Ready();
        selectButtonX = 0;
        selectButtonY = 0;
        Node3D buttonMap = this.GetNode<Node3D>("ButtonMap");
        if(buttonMap == null) throw new ArgumentException("Button Map not found for Shop Interface: " + Name);
        Godot.Collections.Array<Node> buttonColumns = buttonMap.GetChildren();
        shopButton = new ShopButton[buttonColumns.Count][];
        for(int i = 0; i < buttonColumns.Count; i++){
            Godot.Collections.Array<Node> currentColumn = buttonColumns[i].GetChildren();
            shopButton[i] = new ShopButton[currentColumn.Count];
            for(int j = 0; j < currentColumn.Count; j++){
                shopButton[i][j] = (ShopButton) currentColumn[j];
            }
        }
        if(DebugBackOutMode != null) backOutMode = DebugBackOutMode;
        gm = this.GetNode<GameMaster>("/root/GameMaster");
        this.Visible = false;
    }

    public override Task StartUp(GameplayMode oldMode){
        selectButtonX = defaultButtonX;
        selectButtonY = defaultButtonY;
        shopCam.Current = true;
        returnMode = null;
        this.Visible = true;
        descritpionTextBox.Text = defaultBoxText;
        descriptionTitleBox.Text = defaultBoxTitle;
        return Task.CompletedTask;
    }

    public override async Task<GameplayMode> RemoteProcess(double delta)
    {
        //TODO - Have the system respond to different selectButton values!
        //Will be null until someone sets it
        return returnMode;
    }

    public override void HandleInput(PlayerInput input)
    {
        base.HandleInput(input);
        switch(input){
            case PlayerInput.Down:
                if(selectButtonY < shopButton[0].Length-1){
                    shopButton[selectButtonX][selectButtonY].SetSelect(false);
                    selectButtonY++; 
                }
                break;
            case PlayerInput.Up:
                if(selectButtonY > 0){
                    shopButton[selectButtonX][selectButtonY].SetSelect(false);
                    selectButtonY--; 
                }    
                break;
            case PlayerInput.Right:
                if(selectButtonY < shopButton.Rank){
                    shopButton[selectButtonX][selectButtonY].SetSelect(false); 
                    selectButtonX++;
                } 
                break;
            case PlayerInput.Left:
                if(selectButtonY > 0){
                    shopButton[selectButtonX][selectButtonY].SetSelect(false);
                    selectButtonX--;
                }    
                break;
            case PlayerInput.Select:
                returnMode = ProcessButtonPress(shopButton[selectButtonX][selectButtonY].GetActivateString());
                break;
            case PlayerInput.Back:
                returnMode = backOutMode;
                break;
        }
        if(input != PlayerInput.None && input != PlayerInput.Select){
            shopButton[selectButtonX][selectButtonY].SetSelect(true);
            descritpionTextBox.Text = shopButton[selectButtonX][selectButtonY].GetDescriptionText();
            descriptionTitleBox.Text = shopButton[selectButtonX][selectButtonY].GetTitleText();
        }
        //Whatever the current button is gets to be visible
    }

    public override Task TransitionOut(){
        this.Visible = false;
        shopButton[selectButtonX][selectButtonY].SetSelect(false);
        return Task.CompletedTask;
    }

    //Return null unless you want to immediately change what GameplayMode we're in...
    protected virtual GameplayMode ProcessButtonPress(string buttonText){
        return null;
    }
}
