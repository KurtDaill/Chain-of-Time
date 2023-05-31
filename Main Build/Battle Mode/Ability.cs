using System;
using Godot;
using System.Collections.Generic;
using static BattleUtilities;
/*
The ability node is 
*/
public partial class Ability : CombatAction
{

    protected Dictionary<double, int> currentDamageChart;

    protected TargetingLogic AbilityTargetingLogic;

    public virtual void SetTargets(Combatant[] proposedTarget){
        target = proposedTarget;
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

    public (Combatant, string) GetAnimationInfo(){
        return (source, name);
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

