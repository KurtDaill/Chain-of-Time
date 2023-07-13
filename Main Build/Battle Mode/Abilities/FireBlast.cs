using Godot;
using System;
using static BattleUtilities;
public partial class FireBlast : EnemyAbility
{
	/*
	[Export(PropertyHint.File)]
	string fireballFilePath;
	PackedScene fireballScene;
	public override void _Ready(){
        name = "Fireblast";
        animation = "Spellcast";
		//[0] is waiting for our animation, [1] is waiting for the fireball projectile to hit
		flagsRequiredToComplete = new bool[]{false, false};
		AbilityTargetingLogic = BattleUtilities.TargetingLogic.Ranged;
		target = new Combatant[1];
		fireballScene = GD.Load<PackedScene>(fireballFilePath);
    }

	public override void Begin(){
		base.Begin();
		PlayCoreAnimation();
	}

	public override void AnimationTrigger(int phase){
		target[0] = parentBattle.GetRoster().GetCombatant(BattleRank.HeroFront);
		//Get Possible Target with the Lowest HP
		foreach(PlayerCombatant com in parentBattle.GetRoster().GetLegalHeroTargets()){ if(com.GetHP() < target[0].GetHP()) target[0] = com; }
		Projectile fireball = fireballScene.Instantiate<Projectile>();
		fireball.Setup(parentBattle, source, 2, target[0], 1);
		fireball.TargetHit += OnFireballHit;
	}

	public void OnFireballHit(){
		target[0].TakeDamage(2);
		flagsRequiredToComplete[1] = true;
	}
	*/
}
