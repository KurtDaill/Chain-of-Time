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
        exploreCamera.Current = false;
        explorePlayer.SetExploreMode(this);
    }
    public override Task StartUp(){
        explorePlayer.Visible = true;
        exploreCamera.Current = true;
        this.Visible = true;
        return Task.CompletedTask;
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
        explorePlayer.Visible = false;
        this.Visible = false;
    }

    public override void HandleInput(PlayerInput input)
    {
        explorePlayer.HandleInput(input);
    }

    public void SetModeOnDeck(GameplayMode mode){
        modeOnDeck = mode;
    }
}