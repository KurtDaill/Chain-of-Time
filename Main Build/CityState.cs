using Godot;
using System;

public partial class CityState : Node
{
    [Export]
    PackedScene cityPrefab;
    PackedScene savedCity;
    City currentCityWithinScene;
	
    [Signal]
	public delegate void CityLoadedEventHandler();

    public override void _Ready(){
        savedCity = cityPrefab;
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

    //Returns whether or not there's a city properly loaded inside of this scene. Used by functions that need to tell whether they can start working with the city
    public bool IsCityLoaded(){
        return IsInstanceValid(currentCityWithinScene) && currentCityWithinScene != null;
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

    //Used in scene transitions that should also kick the game into the night defense mode
    public void SetNextCityLoadToBeNightDefesnse(){
        this.CityLoaded += ForceNightDefense;
    }

    private void ForceNightDefense(){
        currentCityWithinScene.StartNight();
        //We have to keep this line here to ensure that every new load of the city doesn't force night. Simmilar Signal rigging would require this kind of post-function decoupling.
        this.CityLoaded -= ForceNightDefense;
    }
}
