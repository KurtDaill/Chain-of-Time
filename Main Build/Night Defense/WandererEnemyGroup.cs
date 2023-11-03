using Godot;
using System;

public partial class WandererEnemyGroup : EnemyGroup{
    
    [Export]
    protected float minimumWaitTime;
    [Export]
    protected float maximumWaitTime;
    [Export]
    protected float wanderRange;
    protected Timer waitBetweenWandersTimer;
    protected Vector3 targetWanderCoordinates;

    public async override void _Ready(){
        base._Ready();
        waitBetweenWandersTimer = new Timer();
        this.AddChild(waitBetweenWandersTimer);
        startingPosition = GlobalPosition;
        //await ToSignal(GetParent().GetNode("Navigation Setup Delay"), Timer.SignalName.Timeout);
        PickNewMovementPoint();
        waiting = false;
    }

    /* Process
            Walk Towards the current waypoint
            If we're at the current waypoint:

                generate a new one: */
    public async override void _Process(double delta){
        if(waiting) return;
        this.Velocity = (navigationWaypoints[currentWaypoint] - GlobalPosition).Normalized() * moveSpeed;
        this.MoveAndSlide();
        if((GlobalPosition - targetWanderCoordinates).Length() < 0.2){
            Velocity = Vector3.Zero;
            var rand = new Random();
            var weight = rand.NextDouble();
            waitBetweenWandersTimer.WaitTime = minimumWaitTime * weight + maximumWaitTime * (1-weight);
            waitBetweenWandersTimer.Start();
            waiting = true;
            await ToSignal(waitBetweenWandersTimer, Timer.SignalName.Timeout);
            PickNewMovementPoint();
            waiting = false;
        }
        if((GlobalPosition - navigationWaypoints[currentWaypoint]).Length() < 0.1){
            currentWaypoint++;
        }
    }
    protected void MoveTowardsRandomPointInRange(){
        var rand = new Random();
        //Generates a point in space within our wander range using some randomization math, then asks the server for the closest point on our navigation map
        navigationMap = GetWorld3D().NavigationMap;
        targetWanderCoordinates = NavigationServer3D.MapGetClosestPoint(navigationMap, new Vector3(startingPosition.X + (float)((rand.NextDouble() * 2) - 1) * wanderRange, 0, startingPosition.Z + (float)((rand.NextDouble() * 2) - 1) * wanderRange));
        navigationWaypoints = NavigationServer3D.MapGetPath(navigationMap, GlobalPosition, targetWanderCoordinates, true);
        currentWaypoint = 0;
    }
    protected override void PickNewMovementPoint(){
        MoveTowardsRandomPointInRange();
    }
}