using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualBasic;

public partial class City : Node3D
{
    List<Building> buildings;
    int buildingsDestroyedAsOfLastNight;
    private Building vandalizedBuildingBeingFoughtOver;

    [Export]
    Node3D spawnPointDirectory;
    [Export]
    DirectionalLight3D morningSun, noonSun, eveningSun, nightMoon;
    [Export]
    Godot.Environment morningEnv, noonEnv, eveningEnv, nightEnv;
    [Export]
    Godot.WorldEnvironment worldEnv;
    [Export]
    NavigationRegion3D enemyNavRegion;

    bool nightMode = false;
    public override void _Ready(){
        buildings = new List<Building>();
        foreach(Node node in this.GetNode("Buildings").FindChildren("*", "StaticBody3D")){
            if(node is Building) buildings.Add(node as Building);
        }
        morningSun.Visible = false;
        noonSun.Visible = false;
        eveningSun.Visible = false;
        nightMoon.Visible = false;
        if(nightMode){
            nightMoon.Visible = true;
            worldEnv.Environment = nightEnv;
        }else{
            switch(GetNode<GameMaster>("/root/GameMaster").GetCurrentTU()){
                case 3:
                    morningSun.Visible = true;
                    worldEnv.Environment = morningEnv;
                    break;
                case 2:
                    noonSun.Visible = true;
                    worldEnv.Environment = noonEnv;
                    break;
                case 1 : case 0:
                    eveningSun.Visible = true;
                    worldEnv.Environment = eveningEnv;
                    break;
                default: nightMoon.Visible = true; worldEnv.Environment = nightEnv; break;
            }
        }
        RunUpdates();
    }

    public int GetNumberOfBuildingsDestroyed(){
        return buildings.Count(x => x.IsDestroyed());
    }

    public int GetTotalBuildings(){
        return buildings.Count;
    }

    public Building GetRandomBuilding(bool canBeDestroyed = false, bool canBeVandalized = false){
        //Exceptions to prevent infinitely looping through some weird edge case...
        if(!buildings.Any(x => !x.IsDestroyed())) throw new Exception("How the heck is every building destroyed and the game is still running enough to call Get Random Building? Fix that.");
        if(!buildings.Any(x => !x.IsBeingVandalized())) throw new Exception("Every building in the city is being vandalized...what happened?!");
        
        Random randomGen = new();
        while(true){
            int randy = randomGen.Next(0, buildings.Count - 1);
            if(!canBeDestroyed){ //If we don't want a destroyed building...
                if(buildings[randy].IsDestroyed()) continue;
            }
            if(!canBeVandalized){
                if(buildings[randy].IsBeingVandalized()) continue;
            }
            return buildings[randy];
        }
    }

    public Node3D GetSpawnPointDirectory(){return spawnPointDirectory;}

    public void DestroyRandomBuilding(){
        GetRandomBuilding().DestroyMe();
    }

    //Returns false if there was no building to repair.
    public bool RepairRandomBuilding(){
        buildings = new List<Building>();
        foreach(Node node in this.GetNode("Buildings").FindChildren("*", "StaticBody3D")){
            if(node is Building) buildings.Add(node as Building);
        }
        if(buildings.Any(x => x.IsDestroyed())){
            ((Building)buildings.Where(x=> x.IsDestroyed()).ToArray()[0]).RepairMe();
            return true;
        }else{
            return false;
        }
    }

    //Called from without when the night is over and we need to have all buildings under attack by vandals be destroyed
    public int RegisterVandals(){
        int result = buildings.Count(x => x.IsBeingVandalized());
        foreach(Building build in buildings.Where(x => x.IsBeingVandalized())){
            build.DestroyMe();
        }
        RunUpdates();
        return result;
    }

    public void RunUpdates(){
        foreach(Building build in buildings) build.UpdateState();
    }

    public void SetBuildingBeingFoughtOver(Building build){
        vandalizedBuildingBeingFoughtOver = build;
    }

    public void EndFightOverBuilding(){
        if(vandalizedBuildingBeingFoughtOver != null){
            vandalizedBuildingBeingFoughtOver.StopVandalism();
        }
        vandalizedBuildingBeingFoughtOver = null;
    }

    public void StartNight(){
        nightMode = true;

        morningSun.Visible = false;
        noonSun.Visible = false;
        eveningSun.Visible = false;
        nightMoon.Visible = true;
        worldEnv.Environment = nightEnv;
        this.GetNode<GameMaster>("/root/GameMaster").SetMode(this.GetNode<SceneConfig>("/root/Scene Config").GetMyNightDefenseMode());
    }
    public void EndNight(){
        nightMode = 

        morningSun.Visible = true;
        noonSun.Visible = false;
        eveningSun.Visible = false;
        morningSun.Visible = false;
        worldEnv.Environment = morningEnv;
    }

    public NavigationRegion3D GetEnemyNavRegion(){
        return enemyNavRegion;
    }
}
