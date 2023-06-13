using Godot;
using System;

public partial class PlayerAbility : Ability
{
	protected int spCost = -1;
	public int GetSPCost(){
		return spCost;
	}


	protected string rulesText = "{<Default Rules Text> [YOU SHOULDN'T SEE THIS IN THE UI]";

	public override void Setup(Combatant proposedSource){
		base.Setup(proposedSource);
		if(!(source is PlayerCombatant)){
			GetTree().Quit();
			throw new BadActionSetupException("Player Abilities can only be setup with Player Combatants!");
		}
	}


	public string GetRulesText(){
		return rulesText;
	}
}
