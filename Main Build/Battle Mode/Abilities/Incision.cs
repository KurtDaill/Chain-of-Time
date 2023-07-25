using Godot;
using System;

public partial class Incision : PlayerSkill
{
	public override void _Ready(){
		base._Ready();
		name = "Incision";
		animation = "Incision";
		align = BattleUtilities.AbilityAlignment.Magic;
		skillType = "Attack"; 
		rulesText = "[textSize]small[center]\nDeal 2 DMG to all enemies in Lucienne's Lane. Refunds SP if no enemies remain in her lane.";
		AbilityTargetingLogic = BattleUtilities.TargetingLogic.MyLaneEnemies;
	}

	public override void Begin(){
		base.Begin();
		PlayCoreAnimation();
		target = SearchForTarget();
	}

	public override void AnimationTrigger(int phase){	
		switch(phase){
			case 0: //Normal Attack Trigger
				foreach(EnemyCombatant en in target){
					en.TakeDamage(2);
				}
				break;
			case 1: //Flourish that corrisponds with regaining SP
				bool allDead = true;
				foreach(EnemyCombatant en in parentBattle.GetRoster().GetCombatantsByLane(source.GetPosition().GetLane(), false, true)){
					if(en != null && en.GetHP() > 0) allDead = false;
				}
				if(allDead) ((PlayerCombatant)source).GainSP(spCost);
				break;
			default:
				throw new IndexOutOfRangeException("Ability Incision Accessed with incorrect phase index!");
		}
	}	
}
