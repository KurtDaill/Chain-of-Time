using Godot;
using System;
using static BattleUtilities;
public partial class PlayerAbility : Ability
{

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

	public virtual (Combatant, BattlePosition)[] GetPositionSwaps(){
		return null;
	}
}
