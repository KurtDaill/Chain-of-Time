using System;
using System.Threading.Tasks;
using Godot;
using static GameplayUtilities;
public partial class ShopInterface : GameplayMode{
    public Node3D[][] shopButton;
    private int selectButtonX, selectButtonY;
    public Label3D descriptionBoxTitleLine, descriptionBoxBody;

    public override void _Ready()
    {
        base._Ready();
        selectButtonX = 0;
        selectButtonY = 0;
    }

    public override Task StartUp(GameplayMode oldMode){
        selectButtonX = 0;
        selectButtonY = 0;
        return Task.CompletedTask;
    }

    public override Task<GameplayMode> RemoteProcess(double delta)
    {
        //TODO - Have the system respond to different selectButton values!
        return null;
    }

    public override void HandleInput(PlayerInput input)
    {
        base.HandleInput(input);
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
    }
}
