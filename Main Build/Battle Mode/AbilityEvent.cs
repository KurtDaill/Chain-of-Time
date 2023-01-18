using Godot;
using System;
using static PMBattleUtilities;

public partial class AbilityEvent : Node{
    
    [Export(PropertyHint.Enum)]
    protected AbilityAlignment alignment = AbilityAlignment.Normal;

    [Export]
    protected Target targeting = Target.Self;
    [Export]
    protected bool canHitFlyingCharacters = false;

    [Export(PropertyHint.Enum)]
    protected EventType eventType = EventType.Damage;

    [Export]
    protected bool affectedByPlayerDefense = false;
    [Export]
    int defenseFactor = 0;

    protected PMCharacter[] targets;
    
    [Export]
    protected int eventValue = 0;
    [Export]
    public bool dealsBonusDamageOnStunned = false; //TODO this was Jam code, check battle ability too
    [Export]
    public int stunBonusDamage = 0;

    //Targets refers to the exact targets that are being hit per effect. The Second array are the effected positions, and the first array assigns those arrays to an event number.
    //protected BattlePos[] positions = new BattlePos[0];


    public override void _Ready()
    {
        base._Ready();
        if(eventType == EventType.Status){
            throw new NotAStatusEventException();
        }
    }

    public void SetTarget(PMBattleAbility parentAbility){
        if(targeting == Target.Self){
             targets = new PMCharacter[]{parentAbility.source};
        }
        else if(targeting == Target.SelectedTarget){
            targets = parentAbility.target;
        }
        else if(targeting == Target.AllHero){
            targets = parentAbility.source.parentBattle.GetPlayerCharacters();
        }
        else if(targeting == Target.AllEnemy){
            targets = parentAbility.source.parentBattle.GetEnemyCharacters(); //TODO Refactor this kind of dependancy relationship visa vi Battles?
        }
        else{
             //Because the bitwise relationship for battle positions is shared between BattlePos and Target, we can cast any of the remaining possible Target to a BattlePos
            targets = new PMCharacter[]{parentAbility.source.parentBattle.PositionLookup((BattlePos)targeting)};
        }
    }
    public int GetValue(){
        return eventValue;
    }
    public EventType GetEventType(){
        return eventType;
    }
    public AbilityAlignment GetAlignment(){
        return alignment;
    }
    public PMCharacter[] GetTargets(){
        return targets;
    }
    public bool CanThisHitFliers(){
        return canHitFlyingCharacters;
    }
    public int GetDefenseFactor(){
        if(affectedByPlayerDefense == false) return 0;
        else{
            if(defenseFactor < 0){
                GD.Print("Defense Factor set to less than 0. Remember that Defense is SUBTRACTED from damage");
                return 0;
            }
            else return defenseFactor;
        }
    }
    public enum Target{
        Self,
        SelectedTarget,
        AllCharacters,
        AllEnemy,
        AllHero,
        HeroThree = 0b_100000,
        HeroTwo = 0b_010000,
        HeroOne = 0b_0001000,
        EnemyOne = 0b_000100,
        EnemyTwo = 0b_000010,
        EnemyThree = 0b_000001,
    }
}


[Serializable]
class NotAStatusEventException : Exception
{
    public NotAStatusEventException() : base(String.Format("Status Effect Event Type Set for Non Status Effect Event. Use the Ability Event Status Node Instead.")) {  }
}
