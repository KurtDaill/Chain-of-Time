using Godot;
using System;
using static BattleUtilities;

public partial class LongShot : PlayerSkill
{
	[Export(PropertyHint.File)]
	string laserFX;
	PackedScene laserFXScene;
	public override void _Ready(){
		base._Ready();
		name = "Long Shot";
		animation = "SilverLongShot";
		spCost = 2;
		align = BattleUtilities.AbilityAlignment.Tech;
		skillType = "Attack"; 
		rulesText = "[textSize]small[center] Deals DMG Based on Enemy Position \n Front - 4 DMG\nMiddle - 3 DMG\n  Back - 2 DMG";
		AbilityTargetingLogic = BattleUtilities.TargetingLogic.SingleTargetEnemy;
		laserFXScene = GD.Load<PackedScene>(laserFX);
	}

	public override void Begin(){
		base.Begin();
		PlayCoreAnimation();
	}

	public override void AnimationTrigger(int phase){	
		switch(target[0].GetPosition().GetRank()){
			case BattleRank.EnemyFront :
				target[0].TakeDamage(4);
				break;
			case BattleRank.EnemyMid :
				target[0].TakeDamage(3);
				break;
			case BattleRank.EnemyBack :
				target[0].TakeDamage(2);
				break;
		}
		SpawnEffectOnTarget(1, laserFXScene, target[0]);
	}	
}
