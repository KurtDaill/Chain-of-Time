using Godot;
using System;
using System.Collections.Generic;


public partial class EnemyGroup : CharacterBody3D
{
    //TODO Handle Movement
    [Export]
    private float minimumWaitTime;
    [Export]
    private float maximumWaitTime;

    private Vector3 targetWanderCoordinates = Vector3.Zero;

    private Dictionary<BattlePosition, Combatant> encounterEnemies = new Dictionary<BattlePosition, Combatant>();

    public override void _Ready(){
        encounterEnemies.Add(new BattlePosition(BattleUtilities.BattleLane.Center, BattleUtilities.BattleRank.EnemyFront), 
        GD.Load<PackedScene>("res://Battle Mode/Enemies/SkeletonGuard.tscn").Instantiate<EnemyCombatant>());
    }

    public override void _Process(double delta){
        //if(Mathf.Abs((GlobalPosition - targetWanderCoordinates).Length()) < 1){
            //Pick a new wander coordinate
        //}
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
            Battle encounter = Battle.InstanceBattle(encounterEnemies, true, GlobalPosition);
            GetNode("/root/Scene Config").AddChild(encounter);
            this.GetNode<GameMaster>("/root/GameMaster").SetMode(encounter);
        }
    }
    /*
        OnAreaEntered
            Check if the thing that entered is a player
            If it is : Start Encounter
    */
}
