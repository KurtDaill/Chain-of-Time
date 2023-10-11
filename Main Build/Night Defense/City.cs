using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class City : Node3D
{
    List<Building> buildings;
    int buildingsDestroyedAsOfLastNight;

    public override void _Ready(){
        foreach(Node node in this.FindChildren("*", "Building")){
            buildings.Add(node as Building);
        }
    }

    public int GetNumberOfBuildingsDestroyed(){
        return buildings.Count(x => x.IsDestroyed());
    }

    public int GetTotalBuildings(){
        return buildings.Count;
    }
}
