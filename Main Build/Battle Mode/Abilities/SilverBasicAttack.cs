using Godot;
using System;
using static BattleUtilities;

public partial class SilverBasicAttack : PlayerAbility
{
public SilverBasicAttack(){	
		name = "SilverBasicAttack";
		animation = "SilverBasicAttack";
		currentDamageChart = new System.Collections.Generic.Dictionary<double, int>()
		{
			{0.49, 1},
			{0.51, 2}
		};

		rulesText = " [center]Ranged \n Silver Deals 1-2 Damage";
		AbilityTargetingLogic = TargetingLogic.Ranged;
	}

	public override void Activate(int phase){
		if(phase != 0){
			throw new BadAbilityExecuteCallException("Ability Animation called for Exectuion Phase that isn't defined!");
		}
		if(target.Length != 1){
			throw new BadActionSetupException("Incorrect Targets for Ability " + this.name + ". Need exaclty one target, have " + target.Length + " instead.");
		}
		//if(target[0].GetPosition() != BattlePosition.EnemyFront && source.GetPosition() != BattlePosition.HeroFront){
			//Ability Fails
			//TODO: Figure out how to handle abilities failing to go off
		//}
		target[0].TakeDamage(GenerateDamageFromChart(currentDamageChart));
	}
}
