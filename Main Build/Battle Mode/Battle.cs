using Godot;
using System;

public partial class Battle : Node
{
	private BattlePhase currentPhase = BattlePhase.StartOfTurn;

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

	public override void _Process(double delta){
		switch(currentPhase){
			/*
				Changes functionality depending on current phase

				StartOfTurn
					-Game executes location logic for downed characters (characters who are downed are either moved to the back or eliminated, all others are "crunched" towards the front)
					-Game Iterates through status effects, allowing each to play it's "start of turn" animation (i.e. a burning character animates through a hit react and the damage numbers being displayed above them)
				*/
			case BattlePhase.StartOfTurn :
				break;
				/*
				PlayerSelectsCommands
					-Game hands over funcionality to a GUI object that allows the player to select what abilities/attacks each character will use, it's appended to the Combat Chain

				PlayerCommandExecute
					-Game Iterates through player attacks, allowing each to play its animation in sequence

				TurnOver
					-Game allows each enemy to calculate what attack it wants to execute, a set ammount of time is forced to pass before enemy attacks

				EnemyCommandExecute
					-Game Iterates through enemy attacks, allowing each to play its animaiton in sequence

				There are 5 phases, but only 3 *kinds* of phases
					1. Iterate Through a known list of animations
					2. TurnOver
					3. Player Selects Commands
			*/	
		}
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
