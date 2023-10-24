using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class City : Node3D
{
    List<Building> buildings;
    int buildingsDestroyedAsOfLastNight;

    public override void _Ready(){
        buildings = new List<Building>();
        foreach(Node node in this.FindChildren("*", "StaticBody3D")){
            if(node is Building) buildings.Add(node as Building);
        }
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

    public void DestroyRandomBuilding(){
        GetRandomBuilding().DestroyMe();
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
}
