using System;
using Godot;
using System.Collections.Generic;
using static BattleUtilities;
using System.Linq;
/*
The ability node is 
*/
public partial class Ability : CombatAction
{

    protected Dictionary<double, int> currentDamageChart;

    protected TargetingLogic AbilityTargetingLogic;
    [Export(PropertyHint.Enum)]
    protected Godot.Collections.Array<BattleRank> enabledRanks;

    public virtual void SetTargets(Combatant[] proposedTarget){
        target = proposedTarget;
        targetPosition = new BattlePosition[proposedTarget.Length];
        for(int i = 0; i < proposedTarget.Length; i++){targetPosition[i] = target[i].GetPosition();}
    }

    public virtual (Combatant, BattlePosition)[] GetPositionSwaps(){
		return null;
	}

    public virtual void SetDamageChart(Dictionary<double, int> proposedDamageChart){
        VerifyDamageChart(proposedDamageChart);
        currentDamageChart = proposedDamageChart;
    }

    public TargetingLogic GetTargetingLogic(){
        return AbilityTargetingLogic;
    }

    protected int GenerateDamageFromChart(Dictionary<double, int> damageChart){
        VerifyDamageChart(damageChart);
        System.Random rando = new System.Random();
        double diceRoll = rando.NextDouble();
        double total = 0;
        foreach(KeyValuePair<double,int> entry in damageChart){
            if(entry.Key + total > diceRoll){
                return entry.Value;
            }else{
                total += entry.Key;
            }
        }
        GetTree().Quit();
        throw new DamageChartException("Something unexpected went wrong with the damage chart, you'll have to debug it...");
    }

    protected void VerifyDamageChart(Dictionary<double, int> damageChart){
        double total = 0;
        foreach(KeyValuePair<double, int> entry in damageChart){
            total += entry.Key;
        }
        if(total != 1){
            GetTree().Quit();
            throw new DamageChartException("Invalid Damage Chart, probablilities don't total to 1!");
        }
    }

    protected void PlayCoreAnimation(){  
		source.GetAnimationPlayer().Play(animation);
        ListenForAnimationFinished();
    }

    public Godot.Collections.Array<BattleRank> GetEnabledRanks(){
        return enabledRanks;
    }

    public (Combatant, string) GetAnimationInfo(){
        return (source, name);
    }

    protected void SpawnEffectOnTarget(int bodyRegion, PackedScene effect, Combatant target){
        CombatFX fx = effect.Instantiate<CombatFX>();
        target.AddCombatFX(fx, bodyRegion);
        //target.AddChild(fx);
    }

    


    protected class DamageChartException : Exception{
        
        public DamageChartException()
        {
        }

        public DamageChartException(string message)
            : base(message)
        {
        }

        public DamageChartException(string message, Exception inner)
            : base(message, inner)
        {
        }
        
    }


}

