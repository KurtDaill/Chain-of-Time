using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static PMBattleUtilities;
public class TargetingMenu : BattleMenu {
    private PMPlayerAbility abilityInQuestion;
    private AudioStreamPlayer targetingErrorSound;
    private List<PMCharacter> plannedTargets;
    private TargetingRule workingRule;
    private bool decisionRequired = true;

    //Because we want to avoid having to add a special case to the parent GUI class for opening this menu, we instead
    //Have the previous menu call this command on the targeting menu for setting up the current target instead
    public void SetAbilityForTargeting(PMPlayerAbility newAbility){
        abilityInQuestion = newAbility;
    }

    public override void _Ready()
    {
        base._Ready();
        targetingErrorSound = GetNode<AudioStreamPlayer>("SelectError");
    }

    //Run when this menu is opened, resets values as needed from previous uses
    public override void OnOpen(PMPlayerCharacter character, PMBattle caller){
        base.OnOpen(character, caller);
        if(abilityInQuestion == null) throw new NotImplementedException();//TODO write custom targetingError Exception
        workingRule = abilityInQuestion.GetTargetingRule();
        plannedTargets = new List<PMCharacter>();
        //Checks if we have to ask the player for input on who to target:
        //If not, we mark the targets and tell handle input to not worry about it, otherwise we use everything in handle input
        if((int)workingRule >= 10){//NOTE: Targeting rules less than ten require some choices, Targeting rules above ten are preset
            decisionRequired = false;
            if(workingRule == TargetingRule.Self){
                    SetPointers(character, caller);
                    plannedTargets = new List<PMCharacter>(){character};
            }else{
                switch(workingRule){
                    case TargetingRule.AllEnemy :
                        SetNewTargets(caller.GetEnemyCharacters(abilityInQuestion.CanTargetFliers()).ToList<PMCharacter>(), caller);
                        break;
                    case TargetingRule.AllHero :
                        SetNewTargets(plannedTargets = caller.GetPlayerCharacters(abilityInQuestion.CanTargetFliers()).ToList<PMCharacter>(), caller);
                        break;
                    case TargetingRule.All :
                        SetNewTargets(plannedTargets = caller.GetCharacters().ToList(), caller);
                        break;
                }
                if(plannedTargets.Count == 0) throw new NotImplementedException(); //TODO write custom targetingError exception, if we've gotten here, there's 
                //a targeting rule we haven't accounted for
            }
        }else{
            decisionRequired = true;
            //Set the initial Target for this attack
            //TODO make the "This attack is super invalid" notification
            switch(workingRule){
                    case TargetingRule.SingleEnemyMelee : //Only legal if we're in the front slot
                        if(character.myPosition != BattlePos.HeroOne){
                            //Do the "This attack is super invalid" notification
                            RejectSelection();
                        }else{
                            SetNewTargets(caller.PositionLookup(BattlePos.EnemyOne), caller);
                        }
                        break;
                    case TargetingRule.SingleEnemyReach : //Only legal if we're in the front or middle slot
                        if(character.myPosition != BattlePos.HeroOne || character.myPosition != BattlePos.HeroTwo){
                            //Do the "This attack is super invalid" notification
                            RejectSelection();
                        }else{
                            SetNewTargets(caller.PositionLookup(BattlePos.EnemyOne), caller);
                        }
                        break;
                    case TargetingRule.SingleEnemyRanged : //Always legal if there's an enemy targetable (we check for targetability later)
                        SetNewTargets(caller.PositionLookup(BattlePos.EnemyOne), caller);
                        break;
                    default : //If we've gotten to this point, it's a hero targeting ability, we know we can just set the current chararcter as default
                        SetNewTargets(character, caller);
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
                        //Melee only has one potential target, therefore you can't switch 'em
                        //Play Nuh-no sound effect
                        break;
                    case TargetingRule.SingleEnemyReach :
                        if(caller.PositionLookup(BattlePos.EnemyTwo) != null && plannedTargets[0].myPosition == BattlePos.EnemyOne 
                        && input == MenuInput.Right && character.myPosition == BattlePos.HeroOne){
                        //You can only target the enemy in the second rank with a reach attack if you're right on the front (reach only gives you 2 slots of range)
                            SetNewTargets(plannedTargets, caller);
                        }
                        else if(plannedTargets[0].myPosition == BattlePos.EnemyTwo && input == MenuInput.Left){
                        //We don't check if enemy one exists because they have to in order for there to still be a battle    
                            SetNewTargets(plannedTargets, caller);
                        }
                        break;
                    case TargetingRule.SingleEnemyRanged :
                        switch(plannedTargets[0].myPosition){
                            case BattlePos.EnemyOne :
                                if(input == MenuInput.Right && caller.PositionLookup(BattlePos.EnemyTwo) != null){
                                    SetNewTargets(caller.PositionLookup(BattlePos.EnemyTwo), caller);
                                }else{
                                    RejectSelection(); //We know input == Left and there's no left target or there's no right target at all
                                }
                                break;
                            case BattlePos.EnemyTwo :
                                if(input == MenuInput.Right && caller.PositionLookup(BattlePos.EnemyThree) != null){
                                    SetNewTargets(caller.PositionLookup(BattlePos.EnemyThree), caller);
                                }
                                else if(input == MenuInput.Left){//We don't check if there's an enemy one because there must be for battle to be happening
                                    SetNewTargets(caller.PositionLookup(BattlePos.EnemyOne), caller);
                                }else{
                                    RejectSelection(); //We know input == Left and there's no left target or there's no right target at all
                                }
                                break;
                            case BattlePos.EnemyThree :
                                if(input == MenuInput.Left){//We don't check if there's an enemy two, because there must be if we're targeting enemy three
                                    SetNewTargets(caller.PositionLookup(BattlePos.EnemyTwo), caller);
                                }
                                break;
                        }
                        break;
                    default ://We know that we're looking at a hero targeting ability, and they all behave the same. Developers should set hero abilities intended
                    //to target heroes to "SingleHeroRanged" for clarity
                        switch(plannedTargets[0].myPosition){
                            case BattlePos.HeroOne :
                                if(input == MenuInput.Right && caller.PositionLookup(BattlePos.HeroTwo) != null){
                                    SetNewTargets(caller.PositionLookup(BattlePos.HeroTwo), caller);
                                }else{
                                    RejectSelection(); //because we know input == Left and there's no left target or there's no right target at all
                                }
                                break;
                            case BattlePos.HeroTwo :
                                if(input == MenuInput.Right && caller.PositionLookup(BattlePos.HeroThree) != null){
                                    SetNewTargets(caller.PositionLookup(BattlePos.HeroThree), caller);
                                }
                                else if(input == MenuInput.Left){//We don't check if there's an Hero one because there must be for battle to be happening
                                    SetNewTargets(caller.PositionLookup(BattlePos.HeroOne), caller);
                                }else{
                                    RejectSelection();//We know input == Left and there's no left target or there's no right target at all
                                }
                                break;
                            case BattlePos.HeroThree :
                                if(input == MenuInput.Left){//We don't check if there's an Hero two, because there must be if we're targeting Hero three
                                    SetNewTargets(caller.PositionLookup(BattlePos.HeroTwo), caller);
                                }
                                break;
                        }
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
                    HidePointers(caller);
                    return result;
                }else{ //Our target isn't legal (Oh noes!)
                    RejectSelection();
                }
        }else if(input == MenuInput.Back){
            abilityInQuestion = null;
            parentGUI.ChangeMenu(-1, character, caller);
            return null;
        }
        return null;
    }

    public void RejectSelection(){
        targetingErrorSound.Play();
    }

    public void SetNewTargets(List<PMCharacter> cList, PMBattle battle){
        plannedTargets = cList;
        SetPointers(cList, battle);
    }

    public void SetNewTargets(PMCharacter ch, PMBattle battle){
        plannedTargets = new List<PMCharacter>(){ch};
        SetPointers(ch, battle);
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

    public void HidePointers(PMBattle battle){
        var empty = new List<PMCharacter>();
        SetPointers(empty, battle);
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