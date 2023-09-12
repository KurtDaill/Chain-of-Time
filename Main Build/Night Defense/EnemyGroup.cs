using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;


public partial class EnemyGroup : CharacterBody3D
{
    //TODO Handle Movement
    [Export]
    private float minimumWaitTime;
    [Export]
    private float maximumWaitTime;
    [Export]
    private float wanderRange;
    [Export]
    private float moveSpeed;
    private int currentWaypoint;
    private Timer waitBetweenWandersTimer;
    private bool waiting;
    private Vector3 startingPosition;
    private Vector3[] navigationWaypoints;
    private Vector3 targetWanderCoordinates;
    private Rid navigationMap;
    private Dictionary<BattlePosition, Combatant> encounterEnemies = new Dictionary<BattlePosition, Combatant>();

    public async override void _Ready(){
        waiting = true;
        waitBetweenWandersTimer = new Timer();
        this.AddChild(waitBetweenWandersTimer);

        encounterEnemies.Add(new BattlePosition(BattleUtilities.BattleLane.Center, BattleUtilities.BattleRank.EnemyFront), 
        GD.Load<PackedScene>("res://Battle Mode/Enemies/SkeletonGuard.tscn").Instantiate<EnemyCombatant>());
        startingPosition = GlobalPosition;
        await ToSignal(GetParent().GetNode("Navigation Setup Delay"), Timer.SignalName.Timeout);
        StartWanderingTowardsNewPoint();
        waiting = false;
    }

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
            StartWanderingTowardsNewPoint();
            waiting = false;
        }
        if((GlobalPosition - navigationWaypoints[currentWaypoint]).Length() < 0.1){
            currentWaypoint++;
        }
    }
    /*
        Enemies should wander around within a preset Area3D, and start an encounter when players contact them
    */
    /*
        Process
            Walk Towards the current waypoint
            If we're at the current waypoint:

                generate a new one:
    */

    public void OnEncounterZoneAreaEntered(Area3D area){
        if(area.GetGroups().Contains("Player")){
            Battle encounter = Battle.InstanceBattle(encounterEnemies, this.GetNode<GameMaster>("/root/GameMaster").GetMode(), true, GlobalPosition);
            GetNode("/root/Scene Config").AddChild(encounter);
            this.GetNode<GameMaster>("/root/GameMaster").SetMode(encounter);
        }
    }
    /*
        OnAreaEntered
            Check if the thing that entered is a player
            If it is : Start Encounter
    */
    private void StartWanderingTowardsNewPoint(){
        var rand = new Random();
        //Generates a point in space within our wander range using some randomization math, then asks the server for the closest point on our navigation map
        navigationMap = GetWorld3D().NavigationMap;
        targetWanderCoordinates = NavigationServer3D.MapGetClosestPoint(navigationMap, new Vector3(startingPosition.X + (float)((rand.NextDouble() * 2) - 1) * wanderRange, 0, startingPosition.Z + (float)((rand.NextDouble() * 2) - 1) * wanderRange));
        navigationWaypoints = NavigationServer3D.MapGetPath(navigationMap, GlobalPosition, targetWanderCoordinates, true);
        currentWaypoint = 0;
    }
}
