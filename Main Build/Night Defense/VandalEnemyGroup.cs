using Godot;
using System;

public partial class VandalEnemyGroup : EnemyGroup{
    City myCity;
    Building targetBuildilng;
    Vector3 targetPosition;

    public override async void _Ready()
    {
        base._Ready();
        Timer startTimer = new Timer();
        this.AddChild(startTimer);
        startTimer.WaitTime = 1;
        startTimer.Start();
        await ToSignal(startTimer, Timer.SignalName.Timeout);
        myCity = GetNode<CityState>("/root/CityState").GetCity();
        PickNewMovementPoint();
        waiting = false;
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if(waiting) return;

        //Sets and applies movement.
        this.Velocity = (navigationWaypoints[currentWaypoint] - GlobalPosition).Normalized() * moveSpeed;
        this.MoveAndSlide();

        //
        if((GlobalPosition - targetPosition).Length() < 0.2){
            //Stop and vandalize!
            waiting = true;
            targetBuildilng.StartVandalism();
        }

        //Moves the Enemy From Waypoint to Waypoint.
        if((GlobalPosition - navigationWaypoints[currentWaypoint]).Length() < 0.1){
            currentWaypoint++;
        }
    }
    
    protected override void PickNewMovementPoint(){
        //Pick a Building
        targetBuildilng = myCity.GetRandomBuilding(false, false);
        //Walk Towards it using the Nav Map
        //Generates a point in space within our wander range using some randomization math, then asks the server for the closest point on our navigation map
        navigationMap = GetWorld3D().NavigationMap;
        targetPosition = NavigationServer3D.MapGetClosestPoint(navigationMap, targetBuildilng.GetVandalismPoint());
        navigationWaypoints = NavigationServer3D.MapGetPath(navigationMap, GlobalPosition, targetPosition, true);
        currentWaypoint = 0;
    }

    public override void OnEncounterZoneAreaEntered(Area3D area){
        if(area.GetGroups().Contains("Player")){
            myCity.SetBuildingBeingFoughtOver(targetBuildilng);
        }
        base.OnEncounterZoneAreaEntered(area);
    }

}