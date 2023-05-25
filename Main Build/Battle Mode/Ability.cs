using System;
using Godot;
using System.Collections.Generic;
using static BattleUtilities;
/*
The ability node is 
*/
public partial class Ability : Node
{
    protected string name;
    protected Combatant[] target;
    protected Combatant source;

    protected Dictionary<double, int> currentDamageChart;

    protected TargetingLogic AbilityTargetingLogic;

    //Should be spawned in as a child of proposedSource
    public virtual void Setup(Combatant proposedSource){
        source = proposedSource;
        if(!source.GetChildren().Contains(this)){
            GetTree().Quit();
            throw new BadAbilitySetupException("Ability must be child of combatant it is being setup with!");
        }
        if(source.HasAnimation(name)){
            GetTree().Quit();
            throw new BadAbilitySetupException("Combatant must have an animation with a name the same as this abilities' name!");
        }
    }

    public virtual void SetTargets(Combatant[] proposedTarget){
        target = proposedTarget;
    }

    public virtual void SetDamageChart(Dictionary<double, int> proposedDamageChart){
        VerifyDamageChart(proposedDamageChart);
        currentDamageChart = proposedDamageChart;
    }

    public CombatEventData GetEventData(){
        return new CombatEventData(name, source);
    }

    public TargetingLogic GetTargetingLogic(){
        return AbilityTargetingLogic;
    }

    public virtual void Execute(int phase){
        //Custom Functionality is added here
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

    protected class BadAbilitySetupException : Exception
    {
        public BadAbilitySetupException()
        {
        }

        public BadAbilitySetupException(string message)
            : base(message)
        {
        }

        public BadAbilitySetupException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    protected class BadAbilityExecuteCallException : Exception
    {
        public BadAbilityExecuteCallException()
        {
        }

        public BadAbilityExecuteCallException(string message)
            : base(message)
        {
        }

        public BadAbilityExecuteCallException(string message, Exception inner)
            : base(message, inner)
        {
        }
        
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

