using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using static BattleUtilities;

public abstract partial class EnemyGroup : CharacterBody3D
{
    //TODO Separate functionality between differing types.
    [Export]
    protected float moveSpeed;
    protected int currentWaypoint;
    protected bool waiting = true;
    protected Vector3 startingPosition;
    protected Vector3[] navigationWaypoints;
    protected Rid navigationMap;
    [Export]
    private Godot.Collections.Array<EnemyPositionsPortable> positionsInOrder;
    [Export(PropertyHint.Enum)]
    private Godot.Collections.Array<EnemyType> enemyTypesInOrder;
    protected Godot.Collections.Dictionary<EnemyPositionsPortable, EnemyType> encounterEnemiesByTypeAndPosition = new Godot.Collections.Dictionary<EnemyPositionsPortable, EnemyType>();
    protected Dictionary<BattlePosition, Combatant> encounterEnemies = new Dictionary<BattlePosition, Combatant>();

    public override void _Ready()
    {
        base._Ready();
        for(int i = 0; i < positionsInOrder.Count; i++){
            encounterEnemiesByTypeAndPosition.Add(positionsInOrder[i], enemyTypesInOrder[i]);
        }
        foreach(KeyValuePair<EnemyPositionsPortable, EnemyType> pair in encounterEnemiesByTypeAndPosition){
            encounterEnemies.Add(BattleUtilities.ConvertPosition(pair.Key), GD.Load<PackedScene>(BattleUtilities.GetPathForEnemyType(pair.Value)).Instantiate<EnemyCombatant>());
        }
    }
    public virtual void OnEncounterZoneAreaEntered(Area3D area){
        if(area.GetGroups().Contains("Player")){
            Battle encounter = Battle.InstanceBattle(encounterEnemies, this.GetNode<GameMaster>("/root/GameMaster").GetMode(), true, GlobalPosition, this);
            encounter.GlobalTransform = this.GetNode<CityState>("/root/CityState").GetCity().GetBattlePointMap().GetClosestBattlePoint(this.GlobalPosition).Transform;
            GetNode("/root/SceneConfig").AddChild(encounter);
            this.GetNode<GameMaster>("/root/GameMaster").SetMode(encounter);
            this.Visible = false;
            this.ProcessMode = ProcessModeEnum.Disabled;
        }
    }
    /*
        OnAreaEntered
            Check if the thing that entered is a player
            If it is : Start Encounter
    */
    protected abstract void PickNewMovementPoint();
    
}
