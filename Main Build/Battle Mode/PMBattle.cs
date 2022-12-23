using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using static PMBattleUtilities;
using static BattleMenu;

public partial class PMBattle : Node
{
	private enum TurnPhase{
		LoadDelay,
		Upkeep,
		PlayerMenu,
		PlayerAction,
		TurnOverPause,
		EnemyAction,
		HandleDefeat
	}

	[Export]
	private NodePath battleGUI;
	private PMBattleGUI gui;

	protected GameMaster master;

	TurnPhase currentPhase;
	TurnPhase returnPhase; //What phase do we return to after we've handled defeats

	List<PMStatus> trackedStatusEffects = new List<PMStatus>();
	Stack<PMStatus> effectStack;

	Queue<PMCharacter> enemyBench = new Queue<PMCharacter>(); //Holds onto what enemies are waiting to join the encounter once there's space
	List<PMCharacter> deadPool = new List<PMCharacter>(); //Holds onto what characters are defeated and currently playing their defeat animations

	//Tracks the amount of damage done this turn by each character in order HeroOne, HeroTwo, HeroThree, EnemyOne, EnemyTwo, EnemyThree
	Dictionary<PMCharacter, int> damageScoreboard = new Dictionary<PMCharacter, int>();
	//Tracks healing in the same way as damageScoreboard
	Dictionary<PMCharacter, int> healingScoreboard = new Dictionary<PMCharacter, int>();
	double timer = 0F;
	public bool heroTauntUp{
		get;
		private set;
	}
	public bool enemyTauntUp{
		get;
		private set;
	}

	protected Queue<PMPlayerAbility> playerAttacks;
	protected Queue<PMEnemyAbility> enemyAttacks;
	public PMBattleRoster roster;

	//Used to track what character we know are defeated, so they don't come up every time we check for defeats
	List<PMPlayerCharacter> knownDownedCharacters = new List<PMPlayerCharacter>();
	public override void _Ready()
	{
		//Normal Goto upkeep functions
		currentPhase = TurnPhase.LoadDelay;
		effectStack = new Stack<PMStatus>(trackedStatusEffects);
		gui = (PMBattleGUI) GetNode(battleGUI);
		heroTauntUp = false;
		enemyTauntUp = false;
		roster = GetNode<PMBattleRoster>("Battle Roster");
		master = GetNode<GameMaster>("/root/GameMaster");
		
		foreach(PMPlayerCharacter player in GetPlayerCharacters()){
			player.SetupGUI(gui.GetNode<ReadoutContainer>("Readouts"));
		}
		/*if(master.IsPartyDataOnFile()){
			LoadPlayerCharactersFromGM();
		}*/
		//master = GameMaster;
	}

	public override void _Process(double delta)
	{
		switch(currentPhase){
			case TurnPhase.LoadDelay :
				if(timer < 1){
					timer += delta;
					return;
				}else{
					timer = 0;
					currentPhase = TurnPhase.Upkeep;
				}
				break;
			case TurnPhase.Upkeep :
				foreach(PMCharacter ch in roster.GetCharacters()){
					ch.NewTurnUpkeep();
				}
				if(effectStack.Count == 0){ //If there's no more effects to resolve, continue
					//TODO Check for Taunts
					if(roster.HandleDefeat()){
						gui.ResetGUIState(roster.GetPlayerCharacters(), this);
						currentPhase = TurnPhase.PlayerMenu;
						playerAttacks = new Queue<PMPlayerAbility>();
					}else{
						currentPhase = TurnPhase.HandleDefeat;
						returnPhase = TurnPhase.PlayerMenu;
					}
					
					break;            
				} 

				if(effectStack.Peek().Execute()){ //Execute the Effect, if it's done...
					if(effectStack.Peek().GetDuration() == 0){
						effectStack.Peek().Expire();
					}
					effectStack.Pop();  //RemoveAt it 
				}
				/*TODO: "Specified Cast Not Valid" on these functions: Check out
				foreach(PMPlayerCharacter character in roster.GetPlayerCharacters()){ //Reset the Player Half of the Scoreboards
					damageScoreboard.RemoveAt(character);
				}
				foreach(PMPlayerCharacter character in roster.GetPlayerCharacters()){
					healingScoreboard.RemoveAt(character);
				}
				*/
				//TODO If Players should die, they do
				break;
			case TurnPhase.PlayerMenu :
				//Send Input to the Battle GUI
				var temp = gui.Execute(GetPlayerInput(), this);
				if(temp != null){
					playerAttacks = temp;
					gui.HideGUI();
					if(temp.Count == 0){
						currentPhase = TurnPhase.TurnOverPause;
					}else{
							if(playerAttacks.Peek() != null) playerAttacks.Peek().Begin();
							currentPhase = TurnPhase.PlayerAction;
					}
				}
				break;
			case TurnPhase.PlayerAction : 
				if(playerAttacks.Peek() == null || playerAttacks.Peek().CheckForCompletion()){//Peek Player Attack Stack, get notice whether the attack is still running or not
					playerAttacks.Dequeue();
					if(playerAttacks.Count == 0){//Is there any more attacks?
						if(roster.HandleDefeat()){
							 currentPhase = TurnPhase.TurnOverPause;   
						}else{
							returnPhase = TurnPhase.TurnOverPause;
							currentPhase = TurnPhase.HandleDefeat;
						}
					}else{
						if(playerAttacks.Peek() != null) playerAttacks.Peek().Begin(); //Start the next attack, the previous attack should have reset itself
					}
				}
				break;
			case TurnPhase.TurnOverPause : 
			/*TODO: "Specified Cast Not Valid" on these functions: Check out 
				foreach(PMEnemyCharacter character in damageScoreboard.Keys){
					damageScoreboard.RemoveAt(character);
				}
				foreach(PMEnemyCharacter character in healingScoreboard.Keys){
					healingScoreboard.RemoveAt(character);
				}
			*/
			
				if(timer < 1){
					timer += delta;
					return;
				}else{
					timer = 0;
				}
			
				//Enemies each run their logic for deciding their attack this turn
				enemyAttacks = new Queue<PMEnemyAbility>();
				foreach(PMEnemyCharacter en in roster.GetEnemyCharacters()){
					var enAb = en.DecideAttack();
					if(enAb != null){
						enemyAttacks.Enqueue(enAb);
					}
				}

				enemyAttacks.Peek().Begin();
				currentPhase = TurnPhase.EnemyAction;
				break;
			case TurnPhase.EnemyAction :
				if(enemyAttacks.Peek().CheckForCompletion()){//Peek Enemy Attack Stack, get notice whether the attack is still running or not
					enemyAttacks.Dequeue();
					if(enemyAttacks.Count == 0){//Is there any more attacks?
						//Setup the status effect stack, then turn it over to the next turn
						effectStack = new Stack<PMStatus>();
						foreach(PMStatus status in trackedStatusEffects){
							if(status == null){ trackedStatusEffects.Remove(status); return; }
							effectStack.Push(status);
						}
						
						if(roster.HandleDefeat()){
							if(effectStack.Count != 0) effectStack.Peek().StartUpkeep();
							currentPhase = TurnPhase.Upkeep;
						}else{
							returnPhase = TurnPhase.Upkeep;
							currentPhase = TurnPhase.HandleDefeat;
						}
					}else{
						enemyAttacks.Peek().Begin(); //Start the next attack, the previous attack should have reset itself
					}
				}
				break;
			case TurnPhase.HandleDefeat :
				if(roster.HandleDefeat()){
					if(GetPlayerCharacters(true, true, true, false).Count() == 0){ //if there are no undefeated heroes
						//End the Battle with a Game Over
						EndBattle(true);
					}
					if(GetEnemyCharacters(true, true, true, false).Count() == 0){
						//End the Battle with Victory
						EndBattle(false);
					}
					if(returnPhase == TurnPhase.PlayerMenu){
						gui.ResetGUIState(roster.GetPlayerCharacters(), this);
						playerAttacks = new Queue<PMPlayerAbility>();
					}
					currentPhase = returnPhase; //Only reaches here if there's still a fight
					//Add something so we're not dependant on the turnover pause to handle death...
				}
				break;
		}
	}

	public virtual void EndBattle(bool gameOver){
		if(gameOver)GD.Print("Game Over");
		else GD.Print("You Win!");
		GetTree().Quit();
	}
	public PMCharacter PositionLookup(BattlePos target){
		var temp = roster.GetSingle(target);
		return roster.GetSingle(target);
	}

	public MenuInput GetPlayerInput(){
		if(Input.IsActionJustPressed("ui_up")){
			return BattleMenu.MenuInput.Up;
		}else if(Input.IsActionJustPressed("ui_right")){
			return BattleMenu.MenuInput.Right;
		}else if(Input.IsActionJustPressed("ui_down")){
			return BattleMenu.MenuInput.Down;
		}else if(Input.IsActionJustPressed("ui_left")){
			return BattleMenu.MenuInput.Left;
		}else if(Input.IsActionJustPressed("ui_back")){
			return BattleMenu.MenuInput.Back;
		}else if(Input.IsActionJustPressed("ui_accept")){
			return BattleMenu.MenuInput.Select;
		}else{
			return BattleMenu.MenuInput.None;
		}
	}

	public void UpdateDamageScoreboard(int damage, PMCharacter character){
		int newTotal = 0;
		damageScoreboard.TryGetValue(character, out newTotal);
		damageScoreboard.Remove(character);
		newTotal += damage;
		damageScoreboard.Add(character, newTotal);
	}

	public void UpdateHealingScoreboard(int heal, PMCharacter character){
		int newTotal = 0;
		healingScoreboard.TryGetValue(character, out newTotal);
		healingScoreboard.Remove(character);
		newTotal += heal;
		healingScoreboard.Add(character, newTotal);
	}

	public PMPlayerCharacter GetPlayerDamageLeader(){
		int lead = 0;
		PMPlayerCharacter leader = null;
		foreach(PMCharacter testCharacter in damageScoreboard.Keys){
			if(testCharacter.GetType() == typeof(PMPlayerCharacter)){
				damageScoreboard.TryGetValue(testCharacter, out var temp);
				if(temp > lead){
					lead = temp;
					leader = (PMPlayerCharacter) testCharacter;
				}
			}
		}
		return leader;
	}

	public PMPlayerCharacter GetPlayerHealingLeader(){
		int lead = 0;
		PMPlayerCharacter leader = null;
		foreach(PMCharacter testCharacter in damageScoreboard.Keys){
			if(testCharacter.GetType() == typeof(PMPlayerCharacter)){
				healingScoreboard.TryGetValue(testCharacter, out var temp);
				if(temp > lead){
					lead = temp;
					leader = (PMPlayerCharacter) testCharacter;
				}
			}
		}
		return leader;
	}

	public PMPlayerCharacter[] GetPlayerCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true, bool includeDefeated = true){
		return roster.GetPlayerCharacters(includeFlying, includeInvisible, includePhasedOut, includeDefeated);
	}

	public PMEnemyCharacter[] GetEnemyCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = false, bool includeDefeated = false){
		return roster.GetEnemyCharacters(includeFlying, includeInvisible, includePhasedOut, includeDefeated);
	}

	public PMCharacter[] GetCharacters(bool includeFlying = true, bool includeInvisible = true, bool includePhasedOut = true, bool includeDefeated = false){
		return roster.GetCharacters(includeFlying, includeInvisible, includePhasedOut, includeDefeated);
	}

	public void LogStatusEffect(PMStatus stat){
		trackedStatusEffects.Add(stat);
	}

	public void StartPositionSwap(BattlePos one, BattlePos two){
		roster.StartPositionSwap(one, two);
	}

	public void FinishPositionSwap(){
		roster.FinishPositionSwap();
		gui.GetNode<ReadoutContainer>("Readouts").Reorder();
	}
}

public static class PMBattleUtilities{

	public static NodePath pathToBattle = "/root";

	public enum AbilityAlignment{
		Normal = 0,
		Magic = 1,
		Tech = 2
	}

	public enum EventType{
		None = 0,
		Damage = 1,
		Status = 2,
		Healing = 3
	}

	public enum EnemyRole{
		Minion = 0,
		Tank = 1,
		Bruiser = 2,
		Artillery = 3,
		Support = 4,
		SquadLeader = 5,
		Boss = 6
	}

	[Flags]
	public enum BattlePos{                                                                                                                                                                        
		HeroOne = 0b_001000,
		HeroTwo = 0b_010000,                                                                            
		HeroThree = 0b_100000,
		EnemyOne = 0b_000100,
		EnemyTwo = 0b_000010,
		EnemyThree = 0b_000001
	}                                                                                                    

	[Flags]
	//NOTE: Targeting rules less than ten require some choices, Targeting rules above ten are preset
	public enum TargetingRule{
		None = 0,
		SingleEnemyMelee = 1,
		SingleEnemyRanged = 2,
		SingleEnemyReach = 3,                                                                                                                         
		SingleHeroMelee = 4,
		SingleHeroRanged = 5,
		SingleHeroReach = 6,
		Self = 10,
		AllEnemy = 18,
		AllHero = 19,
		All = 20
	}

	/*
	public BattlePos TargetingRuleToBattlePos(TargetingRule rule){
		switch(rule){
			//case 
		}
	}
	*/

	[Flags]
	public enum StatusEffect{
		None,
		Burn,
		Freeze,
		Poisoned,
		Stunned,
		Flying,
		Silenced,
		Jinxed,
		Overcharged,
		Empowered,
		Invisible,
		PhasedOut,
		Taunting
	}
}
