using Godot;
using System;

public partial class CityState : Node
{
    PackedScene savedCity;
    City currentCityWithinScene;
	
    [Signal]
	public delegate void CityLoadedEventHandler();

    public override void _Ready(){
        savedCity = GD.Load<PackedScene>("res://Explore Mode/DefaultCity.tscn");
    }

    public void SaveCity(City city){
        PackedScene newSaveState = new();
        newSaveState.Pack(city);
        savedCity = newSaveState;
    }

    public void InstantiateSavedCityAtPoint(Marker3D instancePoint){
        currentCityWithinScene = savedCity.Instantiate() as City;
        instancePoint.GetParent().AddChild(currentCityWithinScene);
        currentCityWithinScene.Transform = instancePoint.Transform;
        GD.Print("City Spawned in Successfully");
        EmitSignal(SignalName.CityLoaded);
    }

    public City GetCity(){
        return currentCityWithinScene;
    }

    public void DespawnCity(){
        currentCityWithinScene.QueueFree();
        currentCityWithinScene = null;
    }

    public bool RepairBuildingOutsideOfCityScene(){
        currentCityWithinScene = savedCity.Instantiate() as City;
        bool result = currentCityWithinScene.RepairRandomBuilding();
        SaveCity(currentCityWithinScene);
        DespawnCity();
        return result;
    }
}
