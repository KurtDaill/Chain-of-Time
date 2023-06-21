using Godot;
using System;
using static BattleUtilities;

public partial class SilverBasicAttack : PlayerAbility
{

	[Export(PropertyHint.File)]
	string laserFX;
	PackedScene laserFXScene;
	public override void _Ready(){
		name = "Basic Atk";
		animation = "SilverBasicAttack";
		currentDamageChart = new System.Collections.Generic.Dictionary<double, int>()
		{
			{0.49, 1},
			{0.51, 2}
		};

		rulesText = " [center]Ranged \n Silver Deals 1-2 Damage";
		AbilityTargetingLogic = TargetingLogic.Ranged;
		laserFXScene = GD.Load<PackedScene>(laserFX);
	}

	public override void Begin(){
		base.Begin();
		PlayCoreAnimation();
	}

	public override void AnimationTrigger(int phase){
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
		SpawnEffectOnTarget(1, laserFXScene, target[0]);
	}
}
