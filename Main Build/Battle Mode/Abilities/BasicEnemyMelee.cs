using Godot;
using System;
using System.Collections.Generic;

public partial class BasicEnemyMelee : EnemyAbility
{
	[Export]
	private string setAnimation;
	[Export]
	private string setName;
	[Export]
	private int[] damage;
	[Export]
	private double[] damageProbs;
	private Dictionary<double, int> damageChart;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		name = setName;
		animation = setAnimation;
		damageChart = new Dictionary<double, int>();
		if(damage.Length != damageProbs.Length) throw new ArgumentException();
		if(damage.Length <= 0 || damageProbs.Length <= 0) throw new ArgumentException();
		for(int i = 0; i < damage.Length; i++){
			damageChart.Add(damageProbs[i], damage[i]);
		}
	}

	public override void Begin(){
		base.Begin();
		PlayCoreAnimation();
	}

	public override void AnimationTrigger(int phase){
        parentBattle.GetRoster().GetCombatant(BattleUtilities.BattleRank.HeroFront).TakeDamage(GenerateDamageFromChart(damageChart));
    }
}
