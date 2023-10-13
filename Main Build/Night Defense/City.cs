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

    public void DestroyRandomBuilding(){
        Random randomGen = new();
        while(true){
            int randy = randomGen.Next(0, buildings.Count - 1);
            //Vandalized or Destroyed Buildings aren't valid targets for random destruction.
            if(buildings[randy].IsDestroyed() || buildings[randy].IsBeingVandalized()){
                continue;
            }
            //If we're here, the building's destruciton is 
            buildings[randy].DestroyMe();
            return;
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
}
