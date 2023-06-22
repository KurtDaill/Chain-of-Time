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

	[Signal]
	public delegate void SwapCompleteEventHandler();

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

	private Node3D[] playerSpots = new Node3D[3];
	private Node3D[] enemySpots = new Node3D[3];

	Battle parent;

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
			if(debugPlayer != null){
				SetPositionNewCharacter(debugPlayer, BattlePosition.HeroFront);
			}
			if(debugPlayer2 != null){
				SetPositionNewCharacter(debugPlayer2, BattlePosition.HeroMid);
			}
			if(debugEnemy1 != null){
				SetPositionNewCharacter(debugEnemy1, BattlePosition.EnemyFront);
			}
			if(debugEnemy2 != null){
				SetPositionNewCharacter(debugEnemy2, BattlePosition.EnemyMid);
			}
			if(debugEnemy3 != null){
				SetPositionNewCharacter(debugEnemy3, BattlePosition.EnemyBack);
			}
		}

		parent = GetParent<Battle>();
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
		EmitSignal(Roster.SignalName.SwapComplete);
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

	public void DelistCharacter(Combatant cha){
		positionData.Remove(cha.GetPosition());
		positionData.Add(cha.GetPosition(), null);
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
		Combatant[] output = new Combatant[3];
		positionData.TryGetValue(BattlePosition.HeroFront, out output[0]);
		positionData.TryGetValue(BattlePosition.HeroMid, out output[1]);
		positionData.TryGetValue(BattlePosition.HeroBack, out output[2]);
		PlayerCombatant[] result = new PlayerCombatant[3];
		for(int i = 0; i < 3; i++) result[i] = (PlayerCombatant) output[i];
		return result.Where(x => x != null).ToArray();
	}
	public EnemyCombatant[] GetAllEnemyCombatants(){
		Combatant[] output = new Combatant[3];
		positionData.TryGetValue(BattlePosition.EnemyFront, out output[0]);
		positionData.TryGetValue(BattlePosition.EnemyMid, out output[1]);
		EnemyCombatant[] result = new EnemyCombatant[3];
		for(int i = 0; i < 3; i++) result[i] = (EnemyCombatant) output[i];
		return result.Where(x => x != null).ToArray();
	}

	public Combatant[] GetAllCombatants(){
		Combatant[] output = new Combatant[6];
		positionData.TryGetValue(BattlePosition.HeroFront, out output[0]);
		positionData.TryGetValue(BattlePosition.HeroMid, out output[1]);
		positionData.TryGetValue(BattlePosition.HeroBack, out output[2]);
		positionData.TryGetValue(BattlePosition.EnemyFront, out output[3]);
		positionData.TryGetValue(BattlePosition.EnemyMid, out output[4]);
		positionData.TryGetValue(BattlePosition.EnemyBack, out output[5]); 
		return output.Where(x => x != null).ToArray();
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
		positionData.Remove(BattlePosition.HeroFront);
		positionData.Add(BattlePosition.HeroFront, playerSpots[0].GetChild<PlayerCombatant>(0));

		positionData.Remove(BattlePosition.HeroMid);
		positionData.Add(BattlePosition.HeroMid, playerSpots[1].GetChild<PlayerCombatant>(0));

		positionData.Remove(BattlePosition.HeroBack);
		positionData.Add(BattlePosition.HeroBack, playerSpots[2].GetChild<PlayerCombatant>(0));

		positionData.Remove(BattlePosition.EnemyFront);
		positionData.Add(BattlePosition.EnemyFront, enemySpots[0].GetChild<EnemyCombatant>(0));

		positionData.Remove(BattlePosition.EnemyMid);
		positionData.Add(BattlePosition.EnemyMid, enemySpots[1].GetChild<EnemyCombatant>(0));

		positionData.Remove(BattlePosition.EnemyBack);
		positionData.Add(BattlePosition.EnemyBack, enemySpots[2].GetChild<EnemyCombatant>(0));
	}

	public async void ClearDead(){
		bool[] enemyStates = new bool[3]{
			GetCombatant(BattlePosition.EnemyFront) != null,
			GetCombatant(BattlePosition.EnemyMid) != null,
			GetCombatant(BattlePosition.EnemyBack) != null
		};
		if(!enemyStates[0] & !enemyStates[1]){
			if(!enemyStates[2]) parent.ConcludeBattle(); //GetTree().Quit();
			SwapCharacters(BattlePosition.EnemyBack, BattlePosition.EnemyFront);
			await ToSignal(this.animPlay, AnimationPlayer.SignalName.AnimationFinished);
		}else if(!enemyStates[1]){//There has to be somebody at the front slot to enter this block
			if(enemyStates[2]){
				SwapCharacters(BattlePosition.HeroBack, BattlePosition.EnemyMid);
				await ToSignal(this.animPlay, AnimationPlayer.SignalName.AnimationFinished);
			}
		}else if(!enemyStates[0]){//There isn't anyone at front, but there is someone at mid
			SwapCharacters(BattlePosition.EnemyMid, BattlePosition.EnemyFront);
			await ToSignal(this.animPlay, AnimationPlayer.SignalName.AnimationFinished);
			if(enemyStates[2]){
				SwapCharacters(BattlePosition.EnemyBack, BattlePosition.EnemyMid);
				await ToSignal(this.animPlay, AnimationPlayer.SignalName.AnimationFinished);
			}

		} 
		bool[] playerStates = new bool[3]{
			GetCombatant(BattlePosition.HeroFront) != null && GetCombatant(BattlePosition.HeroFront).GetHP() > 0,
			GetCombatant(BattlePosition.HeroMid) != null && GetCombatant(BattlePosition.HeroMid).GetHP() > 0,
			GetCombatant(BattlePosition.HeroBack) != null && GetCombatant(BattlePosition.HeroBack).GetHP() > 0
		};

		if(!playerStates[0] & !playerStates[1]){
			if(!playerStates[2]) parent.DefeatPlayers();
			SwapCharacters(BattlePosition.HeroBack, BattlePosition.HeroFront);
			await ToSignal(this.animPlay, AnimationPlayer.SignalName.AnimationFinished);
		}else if(!playerStates[1]){//There has to be somebody at the front slot to enter this block
			if(playerStates[2]){
				SwapCharacters(BattlePosition.HeroBack, BattlePosition.HeroMid);
				await ToSignal(this.animPlay, AnimationPlayer.SignalName.AnimationFinished);
			}
		}else if(!playerStates[0]){//There isn't anyone at front, but there is someone at mid
			SwapCharacters(BattlePosition.HeroMid, BattlePosition.HeroFront);
			await ToSignal(this.animPlay, AnimationPlayer.SignalName.AnimationFinished);
			if(playerStates[2]){
				SwapCharacters(BattlePosition.HeroBack, BattlePosition.HeroMid);
				await ToSignal(this.animPlay, AnimationPlayer.SignalName.AnimationFinished);
			}
		} 
	}

	public EnemyCombatant[] GetLegalEnemyTargets(bool ignoresTaunt = false){
		Combatant[] coms = CheckTargetLegality(GetAllEnemyCombatants(), ignoresTaunt);
		EnemyCombatant[] output = new EnemyCombatant[coms.Length];
		for(int i = 0; i < output.Length; i++) output[i] = (EnemyCombatant)coms[i];
		return output;
	}

	public PlayerCombatant[] GetLegalHeroTargets(bool ignoresTaunt = false){
		Combatant[] coms = CheckTargetLegality(GetAllPlayerCombatants(), ignoresTaunt);
		PlayerCombatant[] output = new PlayerCombatant[coms.Length];
		for(int i = 0; i < output.Length; i++) output[i] = (PlayerCombatant)coms[i];
		return output;
	}

	private Combatant[] CheckTargetLegality(Combatant[] check, bool ignoresTaunt){
		List<Combatant> taunters = new List<Combatant>();
		//Adds every character with a taunting status effect to the taunters list
		foreach(Combatant com in check) if(com.GetStatusEffects().Where(x => x is StatusTaunting).Count() != 0) taunters.Add(com);
		if(!ignoresTaunt && taunters.Count() != 0) return taunters.ToArray();
		else return check.Where(x => x.GetHP() > 0).ToArray();
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



