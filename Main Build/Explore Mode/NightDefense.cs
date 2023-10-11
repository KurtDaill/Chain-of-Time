using Godot;
using Godot.Collections;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
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
    private float remainingLight;
    private float playerLampStartingBrightness;

    public override void _Ready(){
        base._Ready();
        remainingLight = lightDurationInSeconds;
        playerLampStartingBrightness = explorePlayer.GetNode<OmniLight3D>("Torchlight").LightEnergy;
    }
    public override async Task<GameplayMode> RemoteProcess(double delta){
        //Mark time, reduce torch count
        remainingLight -= (float)delta;
        remainingLight = Math.Max(0, remainingLight);
        explorePlayer.GetNode<OmniLight3D>("Torchlight").LightEnergy = playerLampStartingBrightness * (remainingLight/lightDurationInSeconds);
        lantern.UpdateLight(remainingLight/lightDurationInSeconds, explorePlayer);
        if(remainingLight == 0){
            //The plan in the full game is to have an entire special phase where the player is hunted down at 0 light, we'll have to see about that.
            //End Night should be called by the following mode (i.e. resultsScreen)
            //EndNight();
        }      
        return base.RemoteProcess(delta).Result;
    }

    //We End the Night.
    public void EndNight(){
        //Idea: Do we just transition to the end of night screen? Have that be it's own gameplay mode?
        foreach(Node node in enemyNavigationRegion.FindChildren("*", "EnemyGroup")){
            node.QueueFree();
        }
    }

    public void BeginNight(){

    }

    public Dictionary<string, int> GetRemainingEnemies(){
        Godot.Collections.Array<Node> enemyGroups = enemyNavigationRegion.FindChildren("*", "EnemyGroup");
        Dictionary<string, int> result = new Dictionary<string, int>();
        result.Add("Wanderer", enemyGroups.Count(x => x is WandererEnemyGroup));
        result.Add("Vandal", enemyGroups.Count(x => x is VandalEnemyGroup));
        return result;
    }
}