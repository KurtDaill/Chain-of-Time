using Godot;
using System;
using System.Collections.Generic;

public partial class Roster : Node
{

	private List<PlayerCombatant> playerCharacters;
	private List<EnemyCombatant> enemyCharacters;

	private Node3D[] playerSpots = new Node3D[3];
	private Node3D[] enemySpots = new Node3D[3];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerSpots[0] = ((Node3D)this.FindChild("PlayerOne"));
		playerSpots[1] = ((Node3D)this.FindChild("PlayerTwo"));
		playerSpots[2] = ((Node3D)this.FindChild("PlayerThree"));

		enemySpots[0] = ((Node3D)this.FindChild("EnemyOne"));
		enemySpots[1] = ((Node3D)this.FindChild("EnemyTwo"));
		enemySpots[2] = ((Node3D)this.FindChild("EnemyThree")); 	

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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void MoveCharacter(Combatant mover, int newSpotIndex){

	}

	public void MoveCharacter(int moverIndex, int newSpotIndex){
		if(moverIndex < 3 && moverIndex >= 0){
			if(enemyCharacters[moverIndex] != null){
				MoveCharacter(enemyCharacters[moverIndex], newSpotIndex);
			}else{
				GetTree().Quit();
				throw new NoCharacterOccupyingSpotAtIndexException();
			}
		}else if (moverIndex < 6){
			if(enemyCharacters[moverIndex - 3] != null){
				MoveCharacter(enemyCharacters[moverIndex - 3], newSpotIndex);
			}else{
				GetTree().Quit();
				throw new NoCharacterOccupyingSpotAtIndexException();
			}
		}else{
			GetTree().Quit();
			throw new IndexOutOfRangeException();
		}
	}

	public PlayerCombatant GetPlayerCombatant(int playerIndex){
		return playerCharacters[playerIndex];
	}

	public EnemyCombatant GetEnemyCombatant(int enemyIndex){
		return enemyCharacters[enemyIndex];
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


