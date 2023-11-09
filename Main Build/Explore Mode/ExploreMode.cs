using System;
using Godot;
using System.Threading.Tasks;
using static GameplayUtilities;
using System.Dynamic;
using static GameMaster;
public partial class ExploreMode : GameplayMode{
    protected ExploreCamera exploreCamera;
    [Export]
    protected ExplorePlayer explorePlayer;
    [Export]
    protected bool CityExplore;

    private GameplayMode modeOnDeck;
    protected Control hud;

    public override void _Ready()
    {
        base._Ready();
        explorePlayer.Visible = false;
        exploreCamera = explorePlayer.GetNode<ExploreCamera>("Explore Camera");
        exploreCamera.Current = false;
        explorePlayer.SetExploreMode(this);
        hud = this.GetNode<Panel>("HUD");
    }
    public override Task StartUp(GameplayMode oldMode){
        explorePlayer.Visible = true;
        exploreCamera.Current = true;
        this.Visible = true;
        hud.Visible = true;
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
        hud.Visible = false;
    }

    public override void HandleInput(PlayerInput input)
    {
        explorePlayer.HandleInput(input);
        if(input == PlayerInput.Start){
            GetNode<GameMaster>("/root/GameMaster").SetMode(GetNode<PauseMenu>("/root/PauseMenu"));
        }
    }

    public void SetModeOnDeck(GameplayMode mode){
        modeOnDeck = mode;
    }

    public OmniLight3D GetCatoLamp(){
        return explorePlayer.GetNode<OmniLight3D>("Torchlight");
    } 

    public void SetExplorePlayerPosition(Vector3 globalPos){
        explorePlayer.GlobalPosition = globalPos;
    }
}