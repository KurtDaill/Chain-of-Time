using Godot;
using Godot.Collections;
using System;
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
    [Export]
    City myCity;
    [Export]
    NavigationRegion3D enemyNavigationRegion;
    [Export]
    ResultsScreen results;
    [Export]
    Marker3D playerNightStartPosition;
    private float remainingLight;
    private float playerLampStartingBrightness;

    private int buildingsDestoryedLastNight = 0;

    public override void _Ready(){
        base._Ready();
        remainingLight = lightDurationInSeconds;
        playerLampStartingBrightness = explorePlayer.GetNode<OmniLight3D>("Torchlight").LightEnergy;
        myCity = GetNode<CityState>("/root/CityState").GetCity();
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
        Dictionary<string,int>enemies = GetRemainingEnemies();
        enemies.TryGetValue("All", out int totalEnemies);
        if(totalEnemies != 0){
            myCity.DestroyRandomBuilding();
            buildingsDestoryedLastNight++;
        }
        buildingsDestoryedLastNight += myCity.RegisterVandals();

        //Clean Up Enemy Groups
        foreach(Node node in enemyNavigationRegion.FindChildren("*", "EnemyGroup")){
            node.QueueFree();
        }

        //TODO Check for Game Over!
    }

    public void BeginNight(){

    }

    public Dictionary<string, int> GetRemainingEnemies(){
        Node[] enemyGroups = enemyNavigationRegion.FindChildren("*", "CharacterBody3D").Where(x => IsInstanceValid(x)).ToArray();
        //If there's a non enemy in there, somethings gone wrong!
        if(enemyGroups.Any(x => !(x is EnemyGroup))) throw new Exception();
        Dictionary<string, int> result = new Dictionary<string, int>();
        result.Add("All", enemyGroups.Length);
        result.Add("Wanderer", enemyGroups.Count(x => x is WandererEnemyGroup));
        result.Add("Vandal", enemyGroups.Count(x => x is VandalEnemyGroup));
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
        if(oldMode is Battle) myCity.EndFightOverBuilding();
        else if(oldMode is ExploreMode || oldMode is PauseMenu){ //Night is beginning from day phase
            explorePlayer.GlobalPosition = playerNightStartPosition.GlobalPosition;
        }
        return result;
    }
}