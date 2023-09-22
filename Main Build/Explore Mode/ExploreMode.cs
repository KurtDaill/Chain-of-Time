using System;
using Godot;
using System.Threading.Tasks;
using static GameplayUtilities;
using System.Dynamic;
using static GameMaster;
public partial class ExploreMode : GameplayMode{
    ExploreCamera exploreCamera;
    [Export]
    ExplorePlayer explorePlayer;
    [Export]
    bool CityExplore;

    private GameplayMode modeOnDeck;

    public override void _Ready()
    {
        base._Ready();
        explorePlayer.Visible = false;
        exploreCamera = explorePlayer.GetNode<ExploreCamera>("Explore Camera");
        exploreCamera.Current = false;
        explorePlayer.SetExploreMode(this);
        GetNode<GameMaster>("/root/GameMaster").TimeOfDayChanged += TimeChange;
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
        if(input == PlayerInput.Select){
            GetNode<GameMaster>("/root/GameMaster").AdvanceClock();
        }
    }

    public void SetModeOnDeck(GameplayMode mode){
        modeOnDeck = mode;
    }

    public void TimeChange(int timeInt){
        string environmentPath;
        TimeOfDay time = (TimeOfDay)timeInt;
        switch(time){
            case TimeOfDay.Morning :
                GetParent().GetNode<DirectionalLight3D>("Night Moon").Visible = false;   
                GetParent().GetNode<DirectionalLight3D>("Morning Sun").Visible = true;
                GetNode<RichTextLabel>("ExploreHUD/Time Label").Text ="Morning";
                environmentPath = "res://Night Defense/Morning Environment.tres";                
                break;
            case TimeOfDay.Noon :
                GetParent().GetNode<DirectionalLight3D>("Morning Sun").Visible = false;   
                GetParent().GetNode<DirectionalLight3D>("Noon Sun").Visible = true;
                GetNode<RichTextLabel>("ExploreHUD/Time Label").Text ="Noon";
                environmentPath = "res://Night Defense/Noon Environment.tres"; 
                break;
            case TimeOfDay.Evening :
                GetParent().GetNode<DirectionalLight3D>("Noon Sun").Visible = false;   
                GetParent().GetNode<DirectionalLight3D>("Sunset").Visible = true;
                GetNode<RichTextLabel>("ExploreHUD/Time Label").Text ="Evening";
                environmentPath = "res://Night Defense/Sunset Environment.tres"; 
                break;
            default :
                throw new Exception("Time not Accounted For!");
        }
        this.GetParent().GetNode<WorldEnvironment>("WorldEnvironment").Environment = GD.Load<Godot.Environment>(environmentPath);
    }
}