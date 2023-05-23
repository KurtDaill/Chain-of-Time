using Godot;
using System;
using System.Threading.Tasks;
public partial class Battle : Node
{
	private BattlePhase currentPhase = BattlePhase.StartOfTurn;

	private (Combatant, string)[] actionChain;

	//private ActionChain theChain = new ActionChain();

	//private List<StatusEffect> Effects;

	[Export]
	private Roster battleRoster;

	public enum BattlePhase{
		StartOfTurn,
		PlayerSelectsCommands,
		PlayerCommandExecute,
		TurnOver,
		EnemyCommandExecute
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
				//Waiting on some guidance on how we're doing status effects
				break;
				
				//PlayerSelectsCommands - Game hands over funcionality to a GUI object that allows the player to select what abilities/attacks each character will use, it's appended to the Combat Chain
			case BattlePhase.PlayerSelectsCommands :
				//GUI.Start Doing your Thing()
				//await ToSignal(GUI is Done)
				break;	
				
			//PlayerCommandExecute -Game Iterates through player attacks, allowing each to play its animation in sequence
			case BattlePhase.PlayerCommandExecute :
				await PlayAnimations(actionChain);
				break;
				
			//TurnOver - Game allows each enemy to calculate what attack it wants to execute, a set ammount of time is forced to pass before enemy attacks
			case BattlePhase.TurnOver :
				EnemyCombatant[] enemies = battleRoster.GetAllEnemyCombatants();
				actionChain = new (Combatant, string)[enemies.Length];
				for(int i = 0; i < enemies.Length; i++){
					actionChain[i] = (enemies[i], enemies[i].DecideAction(this));
				}
				//Delay to add some time between player and enemy attacks?
				break;

			//EnemyCommandExecute - Game Iterates through enemy attacks, allowing each to play its animaiton in sequence
			case BattlePhase.EnemyCommandExecute :
				await PlayAnimations(actionChain);
				break;	
		}		
	}

	//General solution for phases that are essentially "Stop everything let these animations play in sequence"
	private async Task PlayAnimations((Combatant, string)[] animations)
	{
		for(int i = 0; i < animations.Length; i++){
			if(animations[i].Item1.HasAnimation(animations[i].Item2)){
				animations[i].Item1.GetAnimationPlayer().Play(animations[i].Item2);
				await ToSignal(animations[i].Item1.GetAnimationPlayer(), "animationFinished");
			}else{	
				GetTree().Quit();
				throw new BadCombatAnimationException("Listed Animation (" + animations[i].Item2 + ") not found on Combatant (" + animations[i].Item1.GetName() + ")");
			}
		}
	}

	private class BadCombatAnimationException : Exception{
		public BadCombatAnimationException(){}
		public BadCombatAnimationException(string message): base(message){}
		public BadCombatAnimationException(string message, Exception inner) : base(message, inner){}
	}
}

public static class BattleUtilities
{
	public enum BattlePosition{
		EnemyBack,
		EnemyMid,
		EnemyFront,
		PlayerFront,
		PlayerMid,
		PlayerBack
	}
}
