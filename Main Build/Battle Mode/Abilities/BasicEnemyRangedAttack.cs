using System;
using System.Collections.Generic;
using Godot;
using static BattleUtilities;

public partial class BasicEnemyRangedAttack : EnemyAbility
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
		target[0] = GetRangedTargetWithLowestHP(parentBattle);
		PlayCoreAnimation();
	}

	public override void AnimationTrigger(int phase){
        target[0].TakeDamage(GenerateDamageFromChart(damageChart));
    }
}