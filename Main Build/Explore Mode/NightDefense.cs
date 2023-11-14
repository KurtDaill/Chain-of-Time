using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
public partial class NightDefense : ExploreMode
{
    [Export]
    private int lightDurationInSeconds;
    [Export]
    NightDefenseLanternGUI lantern;
    City myCity;
    NavigationRegion3D enemyNavigationRegion;
    [Export]
    ResultsScreen results;
    [Export]
    Marker3D playerNightStartPosition;

    //TODO Make this value load per-night from some kind of central plan?
    [Export(PropertyHint.File)]
    Godot.Collections.Array<NightAttack> nights;
    List<Marker3D> spawnPoints;
    private float remainingLight;
    private float playerLampStartingBrightness;

    private int buildingsDestoryedLastNight = 0;

    protected EnemyGroup[] enemyGroupsInCity;

    public override async void _Ready(){
        base._Ready();
        remainingLight = lightDurationInSeconds;
        playerLampStartingBrightness = explorePlayer.GetNode<OmniLight3D>("Torchlight").LightEnergy;
        myCity = GetNode<CityState>("/root/CityState").GetCity();
        if(myCity == null){ //We failed a load because CityState hasn't been told to load the city yet, let's wait for it.
            await ToSignal(GetNode<CityState>("/root/CityState"), CityState.SignalName.CityLoaded);
            myCity = GetNode<CityState>("/root/CityState").GetCity();
        }
        enemyNavigationRegion = myCity.GetEnemyNavRegion();
        
        hud = this.GetNode<Control>("HUD");
        hud.Visible = false;
    }

    public override async Task<GameplayMode> RemoteProcess(double delta){
        //Mark time, reduce torch count
        remainingLight -= (float)delta;
        remainingLight = Math.Max(0, remainingLight);
        explorePlayer.GetNode<OmniLight3D>("Torchlight").LightEnergy = playerLampStartingBrightness * (remainingLight/lightDurationInSeconds);
        lantern.UpdateLight(remainingLight/lightDurationInSeconds, explorePlayer);
        if(remainingLight == 0){
            SetModeOnDeck(results);
            //The plan in the full game is to have an entire special phase where the player is hunted down at 0 light, we'll have to see about that.
            //End Night should be called by the following mode (i.e. resultsScreen)
            //EndNight();
        }      
        return base.RemoteProcess(delta).Result;
    }

    //We End the Night.
    public void EndNight(){
        buildingsDestoryedLastNight = 0;
        //Night Damage Logic
        System.Collections.Generic.Dictionary<string,int>enemies = GetRemainingEnemies();
        enemies.TryGetValue("All", out int totalEnemies);
        if(totalEnemies != 0){
            myCity.DestroyRandomBuilding();
            buildingsDestoryedLastNight++;
        }
        buildingsDestoryedLastNight += myCity.RegisterVandals();

        //Clean Up Enemy Groups
        foreach(EnemyGroup enemy in enemyGroupsInCity){
            enemy.QueueFree();
        }
        explorePlayer.GetNode<OmniLight3D>("Torchlight").Visible = false;
        //TODO Check for Game Over!
    }

    public void BeginNight(){
        myCity = this.GetNode<CityState>("/root/CityState").GetCity();
        spawnPoints = myCity.GetEnemySpawnPoints();
        enemyNavigationRegion = myCity.GetEnemyNavRegion();
        explorePlayer.GlobalPosition = playerNightStartPosition.GlobalPosition;
        //Go through list of enemy groups
        List<EnemyGroup> thisNightsEnemies = new List<EnemyGroup>();
        foreach(PackedScene group in nights[GetNode<GameMaster>("/root/GameMaster").GetNightNumber()-1].GetEnemyGroups()){
            thisNightsEnemies.Add((EnemyGroup)group.Instantiate());
        }
        //Copy the List of Spawn Points & Randomize the list order
        Random rng = new Random();
        List<Marker3D> spawnPointsRandomized = spawnPoints.Where(x => x != null).OrderBy(x => rng.Next()).ToList();
        
        //throw any error if there aren't enough spawn points
        if(spawnPointsRandomized.Count < thisNightsEnemies.Count){
            GetTree().Quit();
            throw new Exception ("There are less spawn points than enemy groups! This city needs more gottdamn enemy groups!");
        }

        //Assign enemy groups to spawn points
        for(int i = 0; i < thisNightsEnemies.Count; i++){
            enemyNavigationRegion.AddChild(thisNightsEnemies[i], true);
            thisNightsEnemies[i].GlobalPosition = spawnPointsRandomized[i].GlobalPosition;
        }
        enemyGroupsInCity = thisNightsEnemies.ToArray();

        
        explorePlayer.GetNode<OmniLight3D>("Torchlight").Visible = true;
    }

    public Dictionary<string, int> GetRemainingEnemies(){
        Dictionary<string, int> result = new Dictionary<string, int>();
        //If there's a non enemy in there, somethings gone wrong!
        if(enemyGroupsInCity.Where(IsInstanceValid).Any()){
            enemyGroupsInCity = enemyGroupsInCity.Where(IsInstanceValid).ToArray();
            result.Add("All", enemyGroupsInCity.Length);
            result.Add("Wanderer", enemyGroupsInCity.Count(x => x is WandererEnemyGroup));
            result.Add("Vandal", enemyGroupsInCity.Count(x => x is VandalEnemyGroup));
        }else{
            result.Add("All", 0);
            result.Add("Wanderer", 0);
            result.Add("Vandal", 0);
        }
        return result;
    }

    public Dictionary<string, int> GetHomeDestructionReport(){
        Dictionary<string, int> result = new Dictionary<string, int>();
        result.Add("Overall", myCity.GetNumberOfBuildingsDestroyed());
        result.Add("Tonight", buildingsDestoryedLastNight);
        result.Add("Remaining", GetNode<GameMaster>("/root/GameMaster").GetNumberOfBuildingsDestroyedForGameOver() - myCity.GetNumberOfBuildingsDestroyed());
        return result;
        //result.Add("remaining")
    }

    public override Task StartUp(GameplayMode oldMode){
        Task result = base.StartUp(oldMode);
        hud.Visible = true;
        if(oldMode is Battle){
            myCity.EndFightOverBuilding();
            enemyGroupsInCity = enemyGroupsInCity.Where(IsInstanceValid).ToArray();
        }
        if(enemyGroupsInCity != null){
            if(enemyGroupsInCity.Any(x => x != null)){
                foreach(EnemyGroup group in enemyGroupsInCity){
                    //Unpauses All Enemny Groups that might've been paused by exiting the night mode
                    group.ProcessMode = ProcessModeEnum.Always;
                }
            }
        }
        else if(oldMode is ExploreMode || oldMode is PauseMenu){ //Night is beginning from day phase
            BeginNight();
        }
        return result;
    }

    public override OmniLight3D GetCatoLamp()
    {
        return base.GetCatoLamp();
    }

    public override Task TransitionOut(){
        hud.Visible = false;
        if(enemyGroupsInCity != null){
            if(enemyGroupsInCity.Any(x => x != null)){
                foreach(EnemyGroup group in enemyGroupsInCity){
                    //Pauses All Enemy Groups while we're focusing on something else (i.e. Combat)
                    group.ProcessMode = ProcessModeEnum.Disabled;
                }
            }
        }
        return base.TransitionOut();
    }

    //Called by any mode that's going to transition back to a running Night Defense Mode, used to tell the mode to end the night early in certain situations: i.e. Losing Combat, hit the End Night button, etc.
    public void SetNightToEndImmediatelyOnLoad(){
        remainingLight = 0;
    }
}