using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using static GameplayUtilities;
public partial class ShopInterface : GameplayMode{
    public ShopButton[][] shopButton;
    private int selectButtonX, selectButtonY;
    [Export]
    private int defaultButtonX = 0;
    [Export]
    private int defaultButtonY = 0;
    public Label3D descriptionBoxTitleLine, descriptionBoxBody;
    [Export]
    private Camera3D shopCam;

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
    }

    public override Task StartUp(GameplayMode oldMode){
        selectButtonX = defaultButtonX;
        selectButtonY = defaultButtonY;
        shopCam.Current = true;
        return Task.CompletedTask;
    }

    public override async Task<GameplayMode> RemoteProcess(double delta)
    {
        //TODO - Have the system respond to different selectButton values!
        
        return null;
    }

    public override void HandleInput(PlayerInput input)
    {
        base.HandleInput(input);
        //Turn off the old button
        shopButton[selectButtonX][selectButtonY].SetSelect(false);
        switch(input){
            case PlayerInput.Down:
                if(selectButtonY < shopButton[0].Length) selectButtonY++; break;
            case PlayerInput.Up:
                if(selectButtonY > 0) selectButtonY--; break;
            case PlayerInput.Right:
                if(selectButtonY < shopButton.Rank) selectButtonX++; break;
            case PlayerInput.Left:
                if(selectButtonY > 0) selectButtonX--; break;
        }
        //Whatever the current button is gets to be visible
        shopButton[selectButtonX][selectButtonY].SetSelect(true);
    }
}
