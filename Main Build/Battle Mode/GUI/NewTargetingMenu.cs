using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static BattleUtilities;

public partial class NewTargetingMenu : BattleMenu {
    PlayerCombatant source;
    PlayerAbility currentAbility;
    List<Combatant> selectedTargets;
    //Used to track which Rank/Lane the player is targeting
    int positionIndex = -1;

    bool invalid = false;
    public override void _Ready(){

    }

    public override void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        
        //Displays the detail of the current attack in some GUI element
        
    }

    //Because we want to avoid having to add a special case to the BattleGUI class for opening this menu, we instead have the previous menu call this command on the targeting menu for setting up the current target instead
    public void SetAbilityForTargeting(PlayerAbility newAbility, PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        currentAbility = newAbility;
        selectedTargets = null;
        
        if(newAbility is PlayerSkill)parentGUI.spSpentByEachCombatant[parentGUI.GetIndexOfCharacterInQuestion()] = ((PlayerSkill)newAbility).GetSPCost();
        
        //Set all variables/flags related to what ability we're currently targetting.
        switch(newAbility.GetTargetingLogic()){
            case TargetingLogic.Self :
                selectedTargets = new List<Combatant>{character};SetPointers(caller, selectedTargets); break;
            case TargetingLogic.SinlgeTargetPlayer : 
                selectedTargets = new List<Combatant>{character}; break;
            case TargetingLogic.SingleTargetEnemy :
                foreach(EnemyCombatant enemy in caller.GetRoster().GetAllEnemyCombatants()){ if(GetLegalTargets(new List<Combatant>(){enemy}, caller).Count() != 0) selectedTargets = new List<Combatant>(){enemy}; }
                break;
            case TargetingLogic.MyLanePlayers : case TargetingLogic.AnyLaneHitsPlayers :
                positionIndex = (int)character.GetPosition().GetLane();
                selectedTargets = caller.GetRoster().GetCombatantsByLane((BattleLane)positionIndex, true, false).ToList();
                break;
            case TargetingLogic.MyLaneEnemies :
                selectedTargets = caller.GetRoster().GetCombatantsByLane(character.GetPosition().GetLane(), false, true).ToList(); break;
            case TargetingLogic.AnyLaneHitsEnemies :
                for(int i = 0; i < 3; i++){
                    if(caller.GetRoster().GetCombatantsByLane((BattleLane)i, false, true).Count() != 0){
                        positionIndex = i;
                        selectedTargets = caller.GetRoster().GetCombatantsByLane((BattleLane)positionIndex, false, true).ToList();
                        break; 
                    }
                }
                break;
            case TargetingLogic.MyRank : case TargetingLogic.PlayerRank :
                positionIndex = (int)character.GetPosition().GetRank();
                selectedTargets = caller.GetRoster().GetCombatantsByRank((BattleRank)positionIndex).ToList();
                SetPointers(caller, selectedTargets);
                break;
            
            case TargetingLogic.EnemyRank :
                positionIndex = 3; //There has to be SOMEONE at the first enemy Rank.
                selectedTargets = caller.GetRoster().GetCombatantsByRank((BattleRank)positionIndex).ToList();
                SetPointers(caller, selectedTargets);
                break;
            case TargetingLogic.AllEnemies :
                selectedTargets = caller.GetRoster().GetAllEnemyCombatants().ToList<Combatant>();
                SetPointers(caller, selectedTargets);
                break;
            case TargetingLogic.AllPlayers :
                selectedTargets = caller.GetRoster().GetAllPlayerCombatants().ToList<Combatant>();
                SetPointers(caller, selectedTargets);
                break;
        }
    }

    public override PlayerAbility HandleInput(MenuInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        //Respond to player input
        /*
            If the ability is a type that has no player input->
                We wait for the player to press select to confirm...
                    When they do : Run Check TargetLegaltiy
                    if we get a thumbs up: Return the Ability with everything configured.
            Else
                Is the player selecting?
                    A Character?
                        Run HandleSelectCharacter()
                    A Lane?
                        Run HandleSelectLane()
                    A Rank?
                        Run HanldeSelectRank()
                Did they press select?
                    Run Check Target Legaltiy
        */
        
        if(input == MenuInput.Back){
            SetPointers(caller);
            parentGUI.ChangeMenu(0, character);
			character.GainSP(parentGUI.spSpentByEachCombatant[parentGUI.GetIndexOfCharacterInQuestion()]);
			parentGUI.spSpentByEachCombatant[parentGUI.GetIndexOfCharacterInQuestion()] = 0;
            return null;
        }
        switch(currentAbility.GetTargetingLogic()){
            //All of these Targeting Types require no input from the player.
            case TargetingLogic.Self: case TargetingLogic.MyRank: case TargetingLogic.MyLanePlayers : case TargetingLogic.MyLaneEnemies : case TargetingLogic.AllPlayers: case TargetingLogic.AllEnemies:
                break;

            case TargetingLogic.SinlgeTargetPlayer : case TargetingLogic.SingleTargetEnemy :
                HandleSelectCharacter(input, character, caller, parentGUI);
                break;
            case TargetingLogic.AnyLaneHitsPlayers: case TargetingLogic.AnyLaneHitsEnemies :
                HandleSelectLane(input, character, caller, parentGUI);
                break;
            case TargetingLogic.PlayerRank: case TargetingLogic.EnemyRank:
                HandleSelectRank(input, character, caller, parentGUI);
                break;
        }
        if(input == MenuInput.Select){
            List<Combatant> legalTargets = GetLegalTargets(selectedTargets, caller, positionIndex);
            if(selectedTargets[0] is EnemyCombatant){
                if(legalTargets.Count() == 0){
                foreach(EnemyCombatant enemy in caller.GetRoster().GetAllEnemyCombatants()){
                    foreach(StatusTaunting taunt in enemy.GetStatusEffects().Where(x => x is StatusTaunting)){
                        taunt.ShowNotification();
                    }
                }
                 RejectSelection(); 
                 return null; 
                }
            }else{ //No Check needed for player targeting abilities, you can always target friendlies
                legalTargets = selectedTargets;
            }
            
            currentAbility.SetTargets(legalTargets.ToArray());
            SetPointers(caller);
            return currentAbility;    
        }
        return null;
    }

    public void HandleSelectCharacter(MenuInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        int selectLane = (int)selectedTargets[0].GetPosition().GetLane();
        int selectRank = (int)selectedTargets[0].GetPosition().GetRank();
        Combatant newSelection;
        switch(input){
            case MenuInput.Right :
            if(selectRank == 5){ RejectSelection(); return; }
            else selectRank++;
            break;
            case MenuInput.Left :
            if(selectRank == 0){ RejectSelection(); return; }
            else selectRank--;
            break;
            case MenuInput.Up :
            if(selectLane == 2){ RejectSelection(); return; } 
            else selectLane++;
            break;
            case MenuInput.Down :
            if(selectLane == 0){ RejectSelection(); return; } 
            else selectLane--;
            break;
        }
        newSelection = caller.GetRoster().GetCombatant((BattleLane)selectLane, (BattleRank)selectRank);
        if(newSelection == null){ RejectSelection(); return; }
        if(currentAbility.GetTargetingLogic() == TargetingLogic.SinlgeTargetPlayer && newSelection is EnemyCombatant){ RejectSelection(); return; }
        if(currentAbility.GetTargetingLogic() == TargetingLogic.SingleTargetEnemy && newSelection is PlayerCombatant){ RejectSelection(); return; }
        selectedTargets = new List<Combatant>(){newSelection};
        SetPointers(caller, selectedTargets);
    }

    public void HandleSelectLane(MenuInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        switch(input){
            case MenuInput.Up :
                if(positionIndex == 2) { RejectSelection(); return;}
                positionIndex ++;
                break;
            case MenuInput.Down :
                if(positionIndex == 0) { RejectSelection(); return;}
                positionIndex --;
                break;
        }
        selectedTargets = caller.GetRoster().GetCombatantsByLane((BattleLane)positionIndex, (currentAbility.GetTargetingLogic() == TargetingLogic.AnyLaneHitsPlayers), (currentAbility.GetTargetingLogic() == TargetingLogic.AnyLaneHitsEnemies)).ToList();
        SetPointers(caller, selectedTargets);
    }

    public void HandleSelectRank(MenuInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        int checkIndex = positionIndex;
        switch(input){
            case MenuInput.Right:
                if(positionIndex == 5) {RejectSelection(); return;};
                checkIndex ++;
                break;
            case MenuInput.Left :
                if(positionIndex == 0) {RejectSelection(); return;};
                checkIndex --;
                break;
        }
        if(checkIndex < 3 && currentAbility.GetTargetingLogic() == TargetingLogic.EnemyRank) {RejectSelection(); return;}
        if(checkIndex > 2 && currentAbility.GetTargetingLogic() == TargetingLogic.PlayerRank) {RejectSelection(); return;}
        //Check Index is valid if we've reached this block
        positionIndex = checkIndex;
        selectedTargets = caller.GetRoster().GetCombatantsByRank((BattleRank)positionIndex).ToList();
        SetPointers(caller, selectedTargets);
    }   

    //We do EVERY check as to whether or not someone is available for targeting: THIS DOES NOT INCLUDE CHECKS ABOUT THE SOURCE CHARACTER
    public List<Combatant> GetLegalTargets(List<Combatant> desiredTargets, Battle battle, int positionIndex = -1){
        List<Combatant> actualTargets = new List<Combatant>();
        foreach(Combatant target in desiredTargets){
            switch(currentAbility.GetTargetingLogic()){

                case TargetingLogic.SinlgeTargetPlayer : //Check Range & valid type (Enemy v Player)
                    if(currentAbility.GetenabledRanks().Contains(target.GetPosition().GetRank()) && target is PlayerCombatant) actualTargets.Add(target);
                    break;
                
                case TargetingLogic.SingleTargetEnemy : //Check Range & valid type (Enemy v Player)
                    if(currentAbility.GetenabledRanks().Contains(target.GetPosition().GetRank()) && target is EnemyCombatant) actualTargets.Add(target);
                    break;

                case TargetingLogic.MyLanePlayers : //Checks targets are in the correct lane & valid type (Enemy v Player)
                    if(target.GetPosition().GetLane() == source.GetPosition().GetLane() && target is PlayerCombatant) actualTargets.Add(target);
                    break;

                case TargetingLogic.MyLaneEnemies : //Checks targets are in the correct lane
                    if(target.GetPosition().GetLane() != source.GetPosition().GetLane() && target is EnemyCombatant) actualTargets.Add(target);
                    break;

                case TargetingLogic.MyRank : //Checks targets are in the correct rank & valid type (Enemy v Player)
                    if(target.GetPosition().GetRank() != source.GetPosition().GetRank()) continue;
                    actualTargets.Add(target);
                    break;

                case TargetingLogic.AnyLaneHitsPlayers : //Checks targets are in the correct lane & valid type (Enemy v Player)
                    if(positionIndex == -1) throw new ArgumentException("Must define a positionIndex for an AnyLane ability");
                    if((int)target.GetPosition().GetLane() == positionIndex && target is PlayerCombatant) actualTargets.Add(target);
                    break; 
                
                case TargetingLogic.AnyLaneHitsEnemies : //Checks targets are in the correct lane & valid type (Enemy v Player)
                    if(positionIndex == -1) throw new ArgumentException("Must define a positionIndex for an AnyLane ability");
                    if((int)target.GetPosition().GetLane() == positionIndex && target is EnemyCombatant) actualTargets.Add(target);
                    break;

                case TargetingLogic.PlayerRank : //Checks targets are in the correct rank & valid type (Enemy v Player)
                    if(positionIndex == -1) throw new ArgumentException("Must define a positionIndex for AnyRank ability");
                    if((int)target.GetPosition().GetLane() == positionIndex && target is PlayerCombatant) actualTargets.Add(target);
                    break;


                case TargetingLogic.EnemyRank : //Checks targets are in the correct rank & valid type (Enemy v Player)
                    if(positionIndex == -1) throw new ArgumentException("Must define a positionIndex for AnyRank ability");
                    if((int)target.GetPosition().GetLane() == positionIndex && target is EnemyCombatant) actualTargets.Add(target);
                    break;

                case TargetingLogic.AllEnemies : //Make checks for certain taget types. Dunno What Yet.
                    actualTargets = desiredTargets;
                    return actualTargets;

                case TargetingLogic.Self :
                case TargetingLogic.AllPlayers :
                    //You can always target your allies or yourself, no check nessicary.
                    return desiredTargets;
            }
        }
        
        Combatant[] legalTargets = battle.GetRoster().GetLegalEnemyTargets();
        for(int i = 0; i < actualTargets.Count(); i++){
            if(!legalTargets.Contains(actualTargets[i])) actualTargets[i] = null;
        }
        actualTargets = actualTargets.Where(x => x != null).ToList(); 
        return actualTargets;
    }

    //Perfroms any of the tasks that corispond with an ability being rejected for targeting
    public void RejectSelection(){

    }
    public void SetPointers(Battle battle, Combatant character){
        SetPointers(battle, new List<Combatant>(){character});
    }
    public void SetPointers(Battle battle, List<Combatant> characters = null){
        if(characters == null) characters = new List<Combatant>();
        foreach(Combatant cha in battle.GetRoster().GetAllCombatants()){
            if(cha != null) 
                if(characters.Contains(cha)){
                    cha.SetTargetGUIElements(true);
                    continue;
                }
            cha.SetTargetGUIElements(false);
        }
    }
}