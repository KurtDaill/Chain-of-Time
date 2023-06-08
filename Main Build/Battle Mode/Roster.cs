using Godot;
using System;
using System.Collections.Generic;
using static BattleUtilities;
using System.Linq;
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
	private PlayerCombatant debugPlayer;

	[Export]
	private PlayerCombatant debugPlayer2;
	[Export]
	private EnemyCombatant debugEnemy1;
	[Export]
	private EnemyCombatant debugEnemy2;
	[Export]
	private EnemyCombatant debugEnemy3;

	private AnimationPlayer animPlay;

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

		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer"); 	

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
			playerCharacters[0] = debugPlayer;
			SetPositionNewCharacter(debugPlayer, BattlePosition.HeroFront);
			playerCharacters[1] = debugPlayer2;
			SetPositionNewCharacter(debugPlayer2, BattlePosition.HeroMid);
			enemyCharacters[0] = debugEnemy1;
			enemyCharacters[1] = debugEnemy2;
			enemyCharacters[2] = debugEnemy3;
			SetPositionNewCharacter(debugEnemy1, BattlePosition.EnemyFront);
			SetPositionNewCharacter(debugEnemy2, BattlePosition.EnemyMid);
			SetPositionNewCharacter(debugEnemy3, BattlePosition.EnemyBack);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	public async void SwapCharacters(BattlePosition moverPos, BattlePosition newPos){
		List<BattlePosition> positions = new List<BattlePosition>(0){moverPos, newPos};
		Node3D moverSpot = GetSpotNode(moverPos);
		Node3D newSpot = GetSpotNode(newPos);

		if(positions.Contains(BattlePosition.HeroFront) && positions.Contains(BattlePosition.HeroMid)) animPlay.Play("SwapHeroFM");
		else if(positions.Contains(BattlePosition.HeroFront) && positions.Contains(BattlePosition.HeroBack)) animPlay.Play("SwapHeroFB");
		else if(positions.Contains(BattlePosition.HeroMid) && positions.Contains(BattlePosition.HeroBack)) animPlay.Play("SwapHeroMB");
		else if(positions.Contains(BattlePosition.EnemyMid) && positions.Contains(BattlePosition.EnemyBack)) animPlay.Play("SwapEnemyMB");
		else if(positions.Contains(BattlePosition.EnemyFront) && positions.Contains(BattlePosition.EnemyBack)) animPlay.Play("SwapEnemyFB");
		else if(positions.Contains(BattlePosition.EnemyFront) && positions.Contains(BattlePosition.EnemyMid)) animPlay.Play("SwapEnemyFM");
		else throw new KeyNotFoundException();
		await ToSignal(animPlay, AnimationPlayer.SignalName.AnimationFinished);

		Combatant comA = GetCombatant(moverPos);
		Combatant comB = GetCombatant(newPos);
		if(comA != null) moverSpot.RemoveChild(comA);
		if(comB != null) newSpot.RemoveChild(comB);
		animPlay.Play("RESET");
		await ToSignal(animPlay, AnimationPlayer.SignalName.AnimationFinished);

		if(comA != null){
			newSpot.AddChild(comA);
			comA.SetPosition(newPos);
		}
		if(comB != null){
			moverSpot.AddChild(comB);
			comB.SetPosition(moverPos);
		}
		SortCharacters();
	}

	public void SwapCharacters(Combatant mover, BattlePosition newPos){
		SwapCharacters(GetPositionOfCombatant(mover), newPos);
	}

	public void SetPositionNewCharacter(Combatant cha, BattlePosition newPos){
		if(positionData.Values.Contains(cha)) throw new RosterNotConfiguredException("Tried Spawning New Character that already exists!");
		positionData.TryGetValue(newPos, out var atPos);
		if(atPos != null) throw new RosterSpotTakenException("Tried Spawning New Character at Battle Position Already filled!");

		positionData.Remove(newPos);
		positionData.Add(newPos, cha);
		cha.SetPosition(newPos);
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
		return playerCharacters.Where(x => x != null).ToArray();
	}
	public EnemyCombatant[] GetAllEnemyCombatants(){
		return enemyCharacters.Where(x => x != null).ToArray();
	}

	public Combatant[] GetAllCombatants(){
		var result = new Combatant[playerCharacters.Length + enemyCharacters.Length];
		Array.Copy(enemyCharacters, result, enemyCharacters.Length);
		Array.Copy(playerCharacters, 0, result, enemyCharacters.Length, playerCharacters.Length);
		return result.Where(x => x != null).ToArray();
	}

	public AnimationPlayer GetAnimationPlayer(){
		return animPlay;
	}

	public Node3D GetSpotNode(BattlePosition pos){
		switch(pos){
			case BattlePosition.HeroFront : return playerSpots[0];
			case BattlePosition.HeroMid : return playerSpots[1];
			case BattlePosition.HeroBack : return playerSpots[2];
			case BattlePosition.EnemyFront : return enemySpots[0];
			case BattlePosition.EnemyMid : return enemySpots[1];
			case BattlePosition.EnemyBack : return enemySpots[2];
			default : throw new ArgumentException();
		}
	}

	public void SortCharacters(){
		playerCharacters[0] = playerSpots[0].GetChild<PlayerCombatant>(0);
		playerCharacters[1] = playerSpots[1].GetChild<PlayerCombatant>(0);
		playerCharacters[2] = playerSpots[2].GetChild<PlayerCombatant>(0);
		enemyCharacters[0] = enemySpots[0].GetChild<EnemyCombatant>(0);
		enemyCharacters[1] = enemySpots[1].GetChild<EnemyCombatant>(0);
		enemyCharacters[2] = enemySpots[2].GetChild<EnemyCombatant>(0);
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

public class RosterSpotTakenException : Exception
{
    public RosterSpotTakenException()
    {
    }

    public RosterSpotTakenException(string message)
        : base(message)
    {
    }

    public RosterSpotTakenException(string message, Exception inner)
        : base(message, inner)
    {
    }
}



