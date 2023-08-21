using System;
using Godot;
using System.Threading.Tasks;
using static GameplayUtilities;

public partial class ExploreMode : GameplayMode{
    [Export]
    Camera3D exploreCamera;
    [Export]
    ExplorePlayer explorePlayer;

    public override void _Ready()
    {
        base._Ready();
        explorePlayer.Visible = false;
        exploreCamera.Visible = false;
    }
    public override Task StartUp(){
        explorePlayer.Visible = true;
        exploreCamera.Visible = true;
        return null;
    }

    public override async Task<GameplayMode> RemoteProcess(double delta)
    {
        return null;
    }

    public async override Task TransitionOut(){
        //Move the Camera Here...
    }

    public override void HandleInput(PlayerInput input)
    {
        explorePlayer.HandleInput();
    }
}