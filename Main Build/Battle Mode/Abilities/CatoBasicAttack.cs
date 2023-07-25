using Godot;
using System;
using static BattleUtilities;
public partial class CatoBasicAttack : PlayerAbility
{
	/*
		Animation has to decide when stuff happens
		So Animation will need to call a function on its combatant
		That function will need data from this ability...
	*/

	public override void _Ready(){
		base._Ready();	
		name = "Basic Attack";
		animation = "CatoBasicAttack";
		currentDamageChart = new System.Collections.Generic.Dictionary<double, int>()
		{
			{0.49, 1},
			{0.51, 2}
		};

		rulesText = "[center]Cato Deals 1-2 Damage";
		AbilityTargetingLogic = TargetingLogic.SingleTargetEnemy;
		enabledRanks = new Godot.Collections.Array<BattleRank>(){BattleRank.HeroFront, BattleRank.EnemyFront};
	}

	public override void AnimationTrigger(int phase){
		if(phase != 0){
			throw new BadAbilityExecuteCallException("Ability Animation called for Exectuion Phase that isn't defined!");
		}
		if(target.Length != 1){
			throw new BadActionSetupException("Incorrect Targets for Ability " + this.name + ". Need exaclty one target, have " + target.Length + " instead.");
		}
		//if(target[0].GetPosition() != BattleRank.EnemyFront && source.GetPosition() != BattleRank.HeroFront){
		//	//Ability Fails
			//TODO: Figure out how to handle abilities failing to go off
		//}
		target[0].TakeDamage(GenerateDamageFromChart(currentDamageChart));
	}

	public override void Begin(){
		base.Begin();
		PlayCoreAnimation();
		target = SearchForTarget();
	}
}
