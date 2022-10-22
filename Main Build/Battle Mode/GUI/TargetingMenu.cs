using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static PMBattleUtilities;
public class TargetingMenu : BattleMenu {
    private PMPlayerAbility abilityInQuestion;

    private List<PMCharacter> plannedTargets;
    private TargetingRule workingRule;
    private bool decisionRequired = true;

    //Because we want to avoid having to add a special case to the parent GUI class for opening this menu, we instead
    //Have the previous menu call this command on the targeting menu for setting up the current target instead
    public void SetAbilityForTargeting(PMPlayerAbility newAbility){
        abilityInQuestion = newAbility;
    }

    //Run when this menu is opened, resets values as needed from previous uses
    public override void OnOpen(PMPlayerCharacter character, PMBattle caller){
        base.OnOpen(character, caller);
        if(abilityInQuestion == null) throw new NotImplementedException();//TODO write custom targetingError Exception
        workingRule = abilityInQuestion.GetTargetingRule();
        //Checks if we have to ask the player for input on who to target:
        //If not, we mark the targets and tell handle input to not worry about it, otherwise we use everything in handle input
        if((int)workingRule >= 10){//NOTE: Targeting rules less than ten require some choices, Targeting rules above ten are preset
            decisionRequired = false;
            if(workingRule == TargetingRule.Self){
                    SetPointers(character, caller);
                    plannedTargets = new List<PMCharacter>{character};
            }else{
                List<PMCharacter> targetPool = new List<PMCharacter>();
                switch(workingRule){
                    case TargetingRule.AllEnemy :
                        targetPool = caller.GetEnemyCharacters(abilityInQuestion.CanTargetFliers()).ToList<PMCharacter>();
                        break;
                    case TargetingRule.AllHero :
                        targetPool = caller.GetPlayerCharacters(abilityInQuestion.CanTargetFliers()).ToList<PMCharacter>();
                        break;
                    case TargetingRule.All :
                        targetPool = caller.GetCharacters().ToList();
                        break;
                }
                if(targetPool.Count == 0) throw new NotImplementedException(); //TODO write custom targetingError exception
                SetPointers(targetPool, caller);
                plannedTargets = targetPool;
            }
        }else{
            decisionRequired = true;
            //Set the initial Target for this attack
            //TODO make the "This attack is super invalid" notification
            switch(workingRule){
                    case TargetingRule.SingleEnemyMelee : //Only legal if we're in the front slot
                        if(character.myPosition != BattlePos.HeroOne){
                            //Do the "This attack is super invalid" notification
                        }else{
                            SetPointers(caller.PositionLookup(BattlePos.EnemyOne), caller);
                            plannedTargets = new List<PMCharacter>{caller.PositionLookup(BattlePos.EnemyOne)};
                        }
                        break;
                    case TargetingRule.SingleEnemyReach : //Only legal if we're in the front or middle slot
                        if(character.myPosition != BattlePos.HeroOne || character.myPosition != BattlePos.HeroTwo){
                            //Do the "This attack is super invalid" notification
                        }else{
                            SetPointers(caller.PositionLookup(BattlePos.EnemyOne), caller);
                            plannedTargets = new List<PMCharacter>{caller.PositionLookup(BattlePos.EnemyOne)};
                        }
                        break;
                    case TargetingRule.SingleEnemyRanged : //Always legal if there's an enemy targetable (we check for targetability later)
                        SetPointers(caller.PositionLookup(BattlePos.EnemyOne), caller);
                        plannedTargets = new List<PMCharacter>{caller.PositionLookup(BattlePos.EnemyOne)};
                        break;
                    default : //If we've gotten to this point, it's a hero targeting ability, we know we can just set the current chararcter as default
                        SetPointers(character, caller);
                        plannedTargets = new List<PMCharacter>{character};
                        break;
                }
        }
    }

    //Handles input from the core Menu Command
    //Returns a new menu in the scenario we have to switch between menus
    public override PMPlayerAbility HandleInput(MenuInput input, PMPlayerCharacter character, PMBattle caller){ 
        
        if(decisionRequired){
            if(input == MenuInput.Right || input == MenuInput.Left){
                switch(workingRule){
                    case TargetingRule.SingleEnemyMelee :
                        //Play "Nuh-Uh" sound effect
                        if(character.myPosition != BattlePos.HeroOne){
                        }
                        break;
                    case TargetingRule.SingleEnemyReach :
                        break;
                    case TargetingRule.SingleEnemyRanged :
                        break;
                    case TargetingRule.SingleHeroMelee :
                        break;
                    case TargetingRule.SingleHeroRanged :
                        break;
                    case TargetingRule.SingleHeroReach :
                        break;
                }
            }
        }

        
        if(input == MenuInput.Select){
                //Need to Double Check if that target is legal!
                if(CheckTargetLegality(plannedTargets, caller, out var realTargets)){
                    abilityInQuestion.SetTargets(realTargets.ToArray<PMCharacter>());
                    var result = abilityInQuestion;
                    abilityInQuestion = null;
                    return result;
                }else{ //Our target isn't legal (Oh noes!)
                    //Play Nuh-No sound
                }
        }else if(input == MenuInput.Back){
            abilityInQuestion = null;
            parentGUI.ChangeMenu(-1, character, caller);
            return null;
        }
        return null;
    }

    public void SetPointers(PMCharacter character, PMBattle battle){
        foreach(PMCharacter ch in battle.GetCharacters()){
            ch.SetPointerVisibility(false);
        }
        character.SetPointerVisibility(true);
    }

    public void SetPointers(List<PMCharacter> characters, PMBattle battle){
        foreach(PMCharacter ch in battle.GetCharacters()){
            if(characters.Contains(ch)) ch.SetPointerVisibility(true);
            else    ch.SetPointerVisibility(false);
        }
    }

    //returns true if there are ANY valid targets
    public bool CheckTargetLegality(List<PMCharacter> desiredTargets, PMBattle battle, out List<PMCharacter> actualTargets){
        actualTargets = new List<PMCharacter>();
        if(workingRule == TargetingRule.Self){ //You can always hit yourself
            actualTargets = desiredTargets;
            return true;
        }
        bool hitsInvisble = (workingRule == TargetingRule.All || workingRule == TargetingRule.AllHero || workingRule == TargetingRule.AllEnemy);
        var legaltargets = battle.GetCharacters(abilityInQuestion.CanTargetFliers(), hitsInvisble, false);
        foreach(PMCharacter character in desiredTargets){
            if(legaltargets.Contains(character)){
                actualTargets.Add(character);
            }
        }
        actualTargets = null;
        return false;
    }
}