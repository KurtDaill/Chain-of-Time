using System;
using Godot;
using System.Threading.Tasks;
using static GameplayUtilities;
using System.Dynamic;

public partial class ExploreMode : GameplayMode{
    [Export]
    Camera3D exploreCamera;
    [Export]
    ExplorePlayer explorePlayer;

    private GameplayMode modeOnDeck;

    public override void _Ready()
    {
        base._Ready();
        explorePlayer.Visible = false;
        exploreCamera.Visible = false;
        explorePlayer.SetExploreMode(this);
    }
    public override Task StartUp(){
        explorePlayer.Visible = true;
        exploreCamera.Visible = true;
        return null;
    }

    public override async Task<GameplayMode> RemoteProcess(double delta)
    {
        if(modeOnDeck != null){
            GameplayMode temp = modeOnDeck;
            modeOnDeck = null;
            return temp;
        }
        return null;
    }

    public async override Task TransitionOut(){
        //Move the Camera Here...
    }

    public override void HandleInput(PlayerInput input)
    {
        explorePlayer.HandleInput(input);
    }

    public void SetModeOnDeck(GameplayMode mode){
        modeOnDeck = mode;
    }
}