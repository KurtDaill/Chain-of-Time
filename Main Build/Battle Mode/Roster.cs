using Godot;
using System;
using System.Collections.Generic;
using static BattleUtilities;
public partial class Roster : Node
{
	private Dictionary<BattlePosition, Combatant> positionData = new Dictionary<BattlePosition, Combatant>{
		{BattlePosition.EnemyBack, null},
		{BattlePosition.EnemyMid, null},
		{BattlePosition.EnemyFront, null},
		{BattlePosition.HeroFront, null},
		{BattlePosition.HeroMid, null},
		{BattlePosition.HeroBack, null}

	};

	[Export]
	private bool debugMode;

	[Export]
	private NodePath debugPlayer;
	private PlayerCombatant[] playerCharacters = new PlayerCombatant[3];
	private EnemyCombatant[] enemyCharacters = new EnemyCombatant[3];

	private Node3D[] playerSpots = new Node3D[3];
	private Node3D[] enemySpots = new Node3D[3];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerSpots[0] = ((Node3D)this.FindChild("HeroFront"));
		playerSpots[1] = ((Node3D)this.FindChild("HeroMid"));
		playerSpots[2] = ((Node3D)this.FindChild("HeroBack"));

		enemySpots[0] = ((Node3D)this.FindChild("EnemyFront"));
		enemySpots[1] = ((Node3D)this.FindChild("EnemyMid"));
		enemySpots[2] = ((Node3D)this.FindChild("EnemyBack")); 	

		foreach(Node3D playerSpot in playerSpots){
			if(playerSpot == null){
				GetTree().Quit();
				throw new RosterNotConfiguredException("Roster missing object defining player standing position! You must have Node3D's labeled PlayerOne, PlayerTwo, & PlayerThree as children of this node");
			} 
		}

		foreach(Node3D enemySpot in enemySpots){
			
			if(enemySpot == null){
				GetTree().Quit();
				throw new RosterNotConfiguredException("Roster missing object defining enemy standing position! You must have Node3D's labeled EnemyOne, EnemyTwo, & EnemyThree as children of this node");
			} 
		}

		if(debugMode){
			playerCharacters[0] = (PlayerCombatant) GetNode(debugPlayer);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	public void MoveCharacter(BattlePosition moverPos, BattlePosition newPos){
		
	}

	public void MoveCharacter(Combatant mover, BattlePosition newPos){
		MoveCharacter(GetPositionOfCombatant(mover), newPos);
	}


	public BattlePosition GetPositionOfCombatant(Combatant query){
			foreach(KeyValuePair<BattlePosition, Combatant> pair in positionData){ if(pair.Value == query) return pair.Key; }
			
			throw new CombatantNotInRosterException("Combatant " + query.GetName() +  " not found!");
	}

	public Combatant GetCombatant(BattlePosition queryPos){
		if(positionData.TryGetValue(queryPos, out var value)){
			return value;
		}else{
			return null;
		}
	}

	public PlayerCombatant[] GetAllPlayerCombatants(){
		var result = new PlayerCombatant[3];
		playerCharacters.CopyTo(result, 0);
		return result;
	}
	public EnemyCombatant[] GetAllEnemyCombatants(){
		var result = new EnemyCombatant[3];
		enemyCharacters.CopyTo(result, 0);
		return result;
	}

	public Combatant[] GetAllCombatants(){
		var result = new Combatant[6];
		Array.Copy(enemyCharacters, result, enemyCharacters.Length);
		Array.Copy(playerCharacters, 0, result, enemyCharacters.Length, playerCharacters.Length);
		return result;
	}
}
	
public class RosterNotConfiguredException : Exception
{
    public RosterNotConfiguredException()
    {
    }

    public RosterNotConfiguredException(string message)
        : base(message)
    {
    }

    public RosterNotConfiguredException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class CombatantNotInRosterException : Exception
{
    public CombatantNotInRosterException()
    {
    }

    public CombatantNotInRosterException(string message)
        : base(message)
    {
    }

    public CombatantNotInRosterException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class NoCharacterOccupyingSpotAtIndexException : Exception
{
    public NoCharacterOccupyingSpotAtIndexException()
    {
    }

    public NoCharacterOccupyingSpotAtIndexException(string message)
        : base(message)
    {
    }

    public NoCharacterOccupyingSpotAtIndexException(string message, Exception inner)
        : base(message, inner)
    {
    }
}


