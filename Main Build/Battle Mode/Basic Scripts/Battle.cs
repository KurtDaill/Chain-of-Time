using Godot;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using static BattleUtilities;
public partial class Battle : Node3D
{
	private BattlePhase currentPhase = BattlePhase.StartOfTurn;
	[Export]
	private BattleGUI gui;
	private CombatEventData[] eventChain;
	private bool waiting = false;
    private Timer turnoverTimer;
	private Timer betweenActionsTimer; 
	[Export(PropertyHint.File)]
	private string nextBattlePath;
	[Export(PropertyHint.File)]
	private string defeatScreenPath;
	[Export]
	bool startingScene = false;

	//private eventChain theChain = new eventChain();

	//private List<StatusEffect> Effects;

	[Export]
	private Roster battleRoster;
	PackedScene nextBattle;
	PackedScene defeatScreen;

	public enum BattlePhase{
		StartOfTurn,
		PlayerSelectsCommands,
		PlayerCommandExecute,
		TurnOver,
		EnemyCommandExecute
	}

	public override void _Ready(){
        turnoverTimer = this.GetNode<Timer>("Turnover Timer");
		betweenActionsTimer = this.GetNode<Timer>("Between Actions Timer");
		if(nextBattlePath != null) nextBattle = GD.Load<PackedScene>(nextBattlePath);
		if(defeatScreenPath != null) defeatScreen = GD.Load<PackedScene>(defeatScreenPath);
		if(!startingScene){
			GameMaster GM = GetNode<GameMaster>("/root/GameMaster");
			GM.LoadPartyData(battleRoster);
		}
	}

	public override async void _Process(double delta){
		switch(currentPhase){
			/*
				Changes functionality depending on current phase

				StartOfTurn
					-Game executes location logic for downed characters (characters who are downed are either moved to the back or eliminated, all others are "crunched" towards the front)
					-Game Iterates through status effects, allowing each to play it's "start of turn" animation (i.e. a burning character animates through a hit react and the damage numbers being displayed above them)
				*/
			case BattlePhase.StartOfTurn :
				//Game Executes Location logic for downed characters
				//Drag all Status Effect Event Data into Action Chain. then await ExecuteCombatEvents(eventChain);
				if(waiting) return;
				waiting = true;
				await CharactersGoDown();
				betweenActionsTimer.Start();
				await ToSignal(betweenActionsTimer, Timer.SignalName.Timeout);
				List<CombatEventData> statusCED = new List<CombatEventData>();
				foreach(Combatant com in battleRoster.GetAllCombatants()){
					foreach(OnUpkeepStatus up in com.GetUpkeepStatusEffects()){
						statusCED.Add(up.GetEventData());
					}
				}
				eventChain = statusCED.ToArray();
				await ExecuteCombatEvents(eventChain);
				waiting = false;
				currentPhase = BattlePhase.PlayerSelectsCommands;
				break;
				
				//PlayerSelectsCommands - Game hands over funcionality to a GUI object that allows the player to select what abilities/attacks each character will use, it's appended to the Combat Chain
			case BattlePhase.PlayerSelectsCommands :
				//Clear Action Chain
				eventChain = null;
				//GUI.Start Doing your Thing()
				if(waiting) return;
				battleRoster.ClearVirutalPositions();
				waiting = true;
				//Might need better solution for finding characters who are Dead
				gui.ResetGUIStateAndStart(battleRoster.GetAllPlayerCombatants().Where(x => x.GetHP() > 0).ToArray());
				await ToSignal(gui, BattleGUI.SignalName.PlayerFinishedCommandInput);
				eventChain = gui.PickUpQueuedActions();
				waiting = false;
				currentPhase = BattlePhase.PlayerCommandExecute;
				break;	
				
			//PlayerCommandExecute -Game Iterates through player attacks, allowing each to play its animation in sequence
			case BattlePhase.PlayerCommandExecute :
				if(waiting) return;
				waiting = true;
				await ExecuteCombatEvents(eventChain);
				waiting = false;
				currentPhase = BattlePhase.TurnOver;
				gui.HideGUI(true, false);
				break;
				
			//TurnOver - Game allows each enemy to calculate what attack it wants to execute, a set ammount of time is forced to pass before enemy attacks
			case BattlePhase.TurnOver :
				if(waiting) return;
				waiting = true;
				await CharactersGoDown();
				EnemyCombatant[] enemies = battleRoster.GetAllEnemyCombatants();
				eventChain = new CombatEventData[enemies.Length];
				for(int i = 0; i < enemies.Length; i++){
					eventChain[i] = enemies[i].DecideAction(this);
				}
				currentPhase = BattlePhase.EnemyCommandExecute;
				//Delay to add some time between player and enemy attacks?
				turnoverTimer.Start();
				await ToSignal(turnoverTimer,  Timer.SignalName.Timeout);
				waiting = false;
				break;

			//EnemyCommandExecute - Game Iterates through enemy attacks, allowing each to play its animaiton in sequence
			case BattlePhase.EnemyCommandExecute :
				if(waiting) return;
				waiting = true;
				await ExecuteCombatEvents(eventChain);
				waiting = false;
				currentPhase = BattlePhase.StartOfTurn;
				break;	
		}		
	}

	//General solution for phases that are essentially "Stop everything let these animations play in sequence"
	private async Task ExecuteCombatEvents(CombatEventData[] eventData)
	{
		for(int i = 0; i < eventData.Length; i++){
			if(eventData[i].GetAnimationName() == null || eventData[i].GetCombatant().HasAnimation(eventData[i].GetAnimationName())){
				if(eventData[i].GetAnimationName() != "NoAction") eventData[i].GetCombatant().ReadyAction(eventData[i].GetAction(), this); //Recheck this
				eventData[i].GetAction().Begin();
				await ToSignal(eventData[i].GetAction(), CombatAction.SignalName.ActionComplete);
				await CharactersGoDown();
				betweenActionsTimer.Start();
				await ToSignal(betweenActionsTimer, Timer.SignalName.Timeout);
			}else{
					GetTree().Quit();
					throw new BadCombatAnimationException("Listed Animation (" + eventData[i].GetAnimationName() + ") not found on Combatant (" + eventData[i].GetCombatant().GetName() + ")");
			}
		}
	}

	private async Task CharactersGoDown(){
		//Every Character who's at 0 or less goes down: plays their animations then despawns or logs that they're down
		bool anyoneDead = false;
		foreach(Combatant com in battleRoster.GetAllCombatants()){
			if(com.GetHP() <= 0 && com.IsAlreadyDefeated() != true){
				com.DefeatMe();
				await ToSignal(com.GetAnimationPlayer(), AnimationPlayer.SignalName.AnimationFinished);
				if(com is EnemyCombatant){
					com.Free();
					battleRoster.DelistCharacter(com);
				}
				anyoneDead = true;
			}
		}
		//We have roster reshuffle them.
		if(anyoneDead) battleRoster.CrushForward();
	}

	public Roster GetRoster(){
		return battleRoster;
	}

	public void ConcludeBattle(){
		if(nextBattlePath != null){
			GetNode<GameMaster>("/root/GameMaster").SavePlayerParty(battleRoster);
			GetTree().ChangeSceneToPacked(nextBattle);
		}else{
			GetTree().Quit();
		}
	}

	public void DefeatPlayers(){
		GetTree().ChangeSceneToPacked(defeatScreen);
	}

	private class BadCombatAnimationException : Exception{
		public BadCombatAnimationException(){}
		public BadCombatAnimationException(string message): base(message){}
		public BadCombatAnimationException(string message, Exception inner) : base(message, inner){}
	}
}

//The Basic Representation of "A Thing that happens" during combat. This could be the "tick" of a status effect, an ability being used, ect.
public partial class CombatEventData : Godot.GodotObject
{
	private string animationName = "<DefaultAnimationName>";
	private Combatant source = null;
	private CombatAction action = null;

	public CombatEventData(string name, Combatant src, CombatAction act){
		this.animationName = name;
		this.source = src;
		this.action = act;
	}

	public string GetAnimationName() { return animationName; }
	public Combatant GetCombatant() { return source; }

	public CombatAction GetAction() { return action; }
}

public static class BattleUtilities
{
	public enum BattleRank{
		HeroBack = 0,
		HeroMid = 1,
		HeroFront = 2,
		EnemyFront = 3,
		EnemyMid = 4,
		EnemyBack = 5
	}

	public enum BattleLane{
		Top = 0,
		Center = 1,
		Bottom = 2
	}

	public enum TargetingLogic{
		Self,
		SinlgeTargetPlayer,
		SingleTargetEnemy,
		MyRank,
		MyLanePlayers,
		MyLaneEnemies,
		AnyLaneHitsPlayers,
		AnyLaneHitsEnemies,
		PlayerRank,
		EnemyRank,
		AllPlayers,
		AllEnemies,
		Special
	}

	public enum AbilityAlignment{
		Normal,
		Magic,
		Tech
	}
}

public class BattlePosition{
	BattleLane lane;
	BattleRank rank;

	public BattlePosition(BattleLane lane, BattleRank rank){
		this.lane = lane;
		this.rank = rank;
	}
	public BattleLane GetLane(){
		return lane;
	}

	public BattleRank GetRank(){
		return rank;
	}
}