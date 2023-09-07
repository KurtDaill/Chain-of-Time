using Godot;
using System;
using System.Collections.Generic;
using static BattleUtilities;
using System.Linq;
public partial class Roster : Node
{

	private Combatant[,] positionData = new Combatant[3,6];

	private VirtualPositionSwap[] virtualSwaps;

	[Export]
	private Node3D positionPointer;
	[Signal]
	public delegate void SwapCompleteEventHandler();

	[Export]
	private bool debugMode;

	private AnimationPlayer animPlay;

	private Node3D[,] characterSpots = new Node3D[3, 6];

	Battle parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		characterSpots[0, 0] = ((Node3D)this.GetNode("HeroBack0"));
		characterSpots[0, 1] = ((Node3D)this.GetNode("HeroMid0"));
		characterSpots[0, 2] = ((Node3D)this.GetNode("HeroFront0"));
		characterSpots[0, 3] = ((Node3D)this.GetNode("EnemyFront0"));
		characterSpots[0, 4] = ((Node3D)this.GetNode("EnemyMid0"));
		characterSpots[0, 5] = ((Node3D)this.GetNode("EnemyBack0"));

		characterSpots[1, 0] = ((Node3D)this.GetNode("HeroBack1"));
		characterSpots[1, 1] = ((Node3D)this.GetNode("HeroMid1"));
		characterSpots[1, 2] = ((Node3D)this.GetNode("HeroFront1"));
		characterSpots[1, 3] = ((Node3D)this.GetNode("EnemyFront1"));
		characterSpots[1, 4] = ((Node3D)this.GetNode("EnemyMid1"));
		characterSpots[1, 5] = ((Node3D)this.GetNode("EnemyBack1"));

		characterSpots[2, 0] = ((Node3D)this.GetNode("HeroBack2"));
		characterSpots[2, 1] = ((Node3D)this.GetNode("HeroMid2"));
		characterSpots[2, 2] = ((Node3D)this.GetNode("HeroFront2"));
		characterSpots[2, 3] = ((Node3D)this.GetNode("EnemyFront2"));
		characterSpots[2, 4] = ((Node3D)this.GetNode("EnemyMid2"));
		characterSpots[2, 5] = ((Node3D)this.GetNode("EnemyBack2"));

		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer"); 
		positionPointer.Visible = false;

		virtualSwaps = new VirtualPositionSwap[9];
		parent = GetParent<Battle>();

		if(debugMode){
			for(int l = 0; l < 3; l++){
				for(int r = 0; r < 6; r++){
					if(characterSpots[l,r].GetChildren().Count() > 0){
						positionData[l,r] = (Combatant) characterSpots[l,r].GetChild(0);
						positionData[l,r].SetPosition(new BattlePosition((BattleLane) l, (BattleRank) r));
					}
				}
			}
		}
	}

	public void SwapCharacters(BattlePosition moverPos, BattlePosition destinationPos){
		//Get the Spots that corripsond with the Ranks/Lanes
		Node3D moverSpot = characterSpots[(int) moverPos.GetLane(), (int)moverPos.GetRank()];
		Node3D newSpot = characterSpots[(int)destinationPos.GetLane(), (int)destinationPos.GetRank()];
		Combatant comA = GetCombatant(moverPos.GetLane(), moverPos.GetRank());
		Combatant comB = GetCombatant(destinationPos.GetLane(), destinationPos.GetRank());
		if(comA == null && comB == null) return;

		//TODO Redesign Character Swap Animation
		if(comA != null) moverSpot.RemoveChild(comA);
		if(comB != null) newSpot.RemoveChild(comB);
		positionData[(int) moverPos.GetLane(), (int)moverPos.GetRank()] = comB;
		positionData[(int)destinationPos.GetLane(), (int)destinationPos.GetRank()] = comA;
		if(comA != null){
			newSpot.AddChild(comA);
			comA.Position = Vector3.Zero;
			comA.SetPosition(destinationPos.GetLane(), destinationPos.GetRank());
		}
		if(comB != null){
			moverSpot.AddChild(comB);
			comB.Position = Vector3.Zero;
			comB.SetPosition(moverPos.GetLane(), moverPos.GetRank());
		}
		CrushForward();
		EmitSignal(Roster.SignalName.SwapComplete);
	}

	public void SwapCharacters(BattleLane moverLane, BattleRank moverRank, BattleLane destinationLane, BattleRank destinationRank){
		SwapCharacters(new BattlePosition(moverLane, moverRank), new BattlePosition(destinationLane, destinationRank));
	}

	public void SwapCharacters(Combatant mover, BattlePosition destination){
		SwapCharacters(mover.GetPosition(), destination);
	}

	public void SetPositionNewCharacter(Combatant cha, BattleRank rank, BattleLane lane){
		foreach(Combatant com in positionData){ if(com == cha) throw new RosterNotConfiguredException("Tried Spawning New Character that already exists!"); }

		if(positionData[(int)lane, (int)rank] != null) throw new RosterSpotTakenException("Tried Spawning New Character at Battle Position Already filled!");

		positionData[(int)lane, (int)rank] = cha;
		characterSpots[(int)lane, (int)rank].AddChild(cha);
		cha.GlobalPosition = characterSpots[(int)lane, (int)rank].GlobalPosition;
		cha.SetPosition(lane, rank);
	}

	public async void SetStartingCharacter(Combatant cha, BattleRank rank, BattleLane lane){
		await ToSignal(this, Node.SignalName.Ready);
		SetPositionNewCharacter(cha, rank, lane);
	}

	public void DelistCharacter(Combatant cha){ positionData[(int)cha.GetPosition().GetLane(), (int)cha.GetPosition().GetRank()] = null; }

	public BattlePosition GetPositionOfCombatant(Combatant query){
		foreach(Combatant com in positionData){ if(com == query) return query.GetPosition(); }
		throw new ArgumentException("Combatant not found in combat");
	}

	public Combatant GetCombatant(BattleLane queryLane, BattleRank queryRank){
		return positionData[(int)queryLane, (int)queryRank];
	}
	public Combatant GetCombatant(BattlePosition position){
		return GetCombatant(position.GetLane(), position.GetRank());
	}

	public PlayerCombatant[] GetAllPlayerCombatants(){
		List<PlayerCombatant> result = new List<PlayerCombatant>();
		for(int i = 0; i < 3; i++){
			for(int j = 2; j >= 0; j--){ result.Add((PlayerCombatant)positionData[i,j]); }
		}
		return result.Where(x => x != null).ToArray();
	}
	
	public EnemyCombatant[] GetAllEnemyCombatants(){
		List<EnemyCombatant> result = new List<EnemyCombatant>();
		for(int i = 0; i < 3; i++){
			for(int j = 3; j < 6; j++){ result.Add((EnemyCombatant)positionData[i,j]); }
		}
		return result.Where(x => x != null).ToArray();
	}
	
	public Combatant[] GetAllCombatants(){
		List<Combatant> result = new List<Combatant>();
		for(int i = 0; i < 3; i++){
			for(int j = 0; j < 6; j++){ result.Add(positionData[i,j]); }
		}
		return result.Where(x => x != null).ToArray();
	}

	public Combatant[] GetCombatantsByLane(BattleLane lane, bool includePlayers, bool includeEnemies){
		Combatant[] result = new Combatant[6];
		for(int i = 0; i < 6; i++){
			result[i] = positionData[(int)lane, i];
			if(!includePlayers && result[i] is PlayerCombatant) result[i] = null;
			if(!includeEnemies && result[i] is EnemyCombatant) result[i] = null;
		}
		return result.Where(x => x != null).ToArray();
	}

	public Combatant[] GetCombatantsByRank(BattleRank rank){
		Combatant[] result = new Combatant[3];
		for(int i = 0; i < 3; i++){
			result[i] = positionData[i, (int)rank];
		}
		return result.Where(x => x != null).ToArray();
	}

	public AnimationPlayer GetAnimationPlayer(){
		return animPlay;
	}

	public void CrushForward(){
		for(int i = 0; i < 3; i++){
			bool[] laneState = new bool[3];
			for(int h = 0; h < 3; h++){if(positionData[i,h] != null && positionData[i,h].GetHP() > 0)laneState[h] = true; else laneState[h] = false;}
			if(!laneState[2]) SwapCharacters((BattleLane)i, BattleRank.HeroMid, (BattleLane)i, BattleRank.HeroFront);
			if(!laneState[1] && laneState[0]) SwapCharacters((BattleLane)i, BattleRank.HeroBack, (BattleLane)i, BattleRank.HeroMid);
		}
		for(int i = 0; i < 3; i++){
			bool[] laneState = new bool[3];
			for(int e = 3; e < 6; e++){if(positionData[i,e] != null && positionData[i,e].GetHP() > 0)laneState[e - 3] = true; else laneState[e - 3] = false;}
			if(!laneState[0]) SwapCharacters((BattleLane)i, BattleRank.EnemyMid, (BattleLane)i, BattleRank.EnemyFront);
			if(!laneState[1] && laneState[2]) SwapCharacters((BattleLane)i, BattleRank.EnemyBack, (BattleLane)i, BattleRank.EnemyMid);
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
		bool bottomLaneHasTaunters = false;
		bool centerLaneHasTaunters = false;
		bool topLaneHasTaunters = false;
		//Adds every character with a taunting status effect to the taunters list
		if(ignoresTaunt) return check;
		foreach(Combatant com in check) if(com.GetStatusEffects().Where(x => x is StatusTaunting).Count() != 0){
			taunters.Add(com);
			switch(com.GetPosition().GetLane()){
				case BattleLane.Bottom : bottomLaneHasTaunters = true; break;
				case BattleLane.Center : centerLaneHasTaunters = true; break;
				case BattleLane.Top : topLaneHasTaunters = true; break;
			}
		}
		if(taunters.Count() == 0) return check.Where(x => x.GetHP() > 0).ToArray();
		else{
			List<Combatant> targetsValidAfterTauntCheck = new List<Combatant>();
			if(!bottomLaneHasTaunters) targetsValidAfterTauntCheck.AddRange(check.Where(x => x.GetPosition().GetLane() == BattleLane.Bottom));
			if(!centerLaneHasTaunters) targetsValidAfterTauntCheck.AddRange(check.Where(x => x.GetPosition().GetLane() == BattleLane.Center));
			if(!topLaneHasTaunters) targetsValidAfterTauntCheck.AddRange(check.Where(x => x.GetPosition().GetLane() == BattleLane.Top));
			targetsValidAfterTauntCheck.AddRange(taunters);
			return targetsValidAfterTauntCheck.ToArray();
		}
	}

	public BattlePosition GetCharacterVirtualPosition(Combatant character){
		Combatant[,] virtualPositions = positionData.Clone() as Combatant[,];
		for(int i = 0; i < 3; i++){
			if(virtualSwaps[i] is VirtualPositionSwap){
				foreach((Combatant, BattlePosition) swap in virtualSwaps[i].GetSwapInstructions()){
					BattlePosition gridStart = null;
					//Sweep through all of the virtual positions to find the target we're looking for
					for(int r = 0; r < 6; r++){for(int l = 0; l < 3; l++){
						if(virtualPositions[l,r] == swap.Item1) gridStart = new BattlePosition((BattleLane)l, (BattleRank)r);
					}}
					if(gridStart == null) throw new NotImplementedException(); //TODO: Custom exception for invalid combatant data.
					BattlePosition gridDestination = swap.Item2;
					Combatant source = swap.Item1;
					Combatant destination = virtualPositions[(int)swap.Item2.GetLane(), (int)swap.Item2.GetRank()];
					virtualPositions[(int)gridStart.GetLane(), (int)gridStart.GetRank()] = destination;
					virtualPositions[(int)gridDestination.GetLane(), (int)gridDestination.GetRank()] = source;
				}
			}
		}
		for(int l  = 0; l < 3; l++){
			for(int r = 0; r < 6; r++){
				if(virtualPositions[l,r] == character) return new BattlePosition((BattleLane)l, (BattleRank)r);
			} 
		}
		throw new ArgumentException("Character not found.");
		//return ((KeyValuePair<BattleRank, Combatant>)virtualPositions.FirstOrDefault(x => x.Value == character)).Key;
	}

	public void ClearVirutalPositions(){
		virtualSwaps = new VirtualPositionSwap[3];
	}

	public void LogVirtualPositionSwap(int index, (Combatant, BattlePosition)[] swaps){
		virtualSwaps[index] = new VirtualPositionSwap(swaps);
	}
	
	public void RollBackVirtualPositionSwap(int index){
		virtualSwaps[index] = null;
	}
	public void HidePointer(){
        positionPointer.Visible = false;
    }
    public void ShowPointer(){
        positionPointer.Visible = true;
    }
	public void SetPointerPosition(int lane, int rank){
		positionPointer.GlobalPosition = characterSpots[lane,rank].GlobalPosition;
	}
}

public class VirtualPositionSwap{
	private (Combatant, BattlePosition)[] swaps;
	public VirtualPositionSwap((Combatant, BattlePosition)[] newSwaps){
		swaps = newSwaps;
	}

	public (Combatant, BattlePosition)[] GetSwapInstructions(){
		return swaps;
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



