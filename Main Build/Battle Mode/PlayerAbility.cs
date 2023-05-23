using Godot;
using System;

public partial class PlayerAbility : Ability
{
	protected int spCost = 0;

	public override void Setup(Combatant proposedSource){
		base.Setup(proposedSource);
		if(!(source is PlayerCombatant)){
			GetTree().Quit();
			throw new BadAbilitySetupException("Player Abilities can only be setup with Player Combatants!");
		}
	}
}
