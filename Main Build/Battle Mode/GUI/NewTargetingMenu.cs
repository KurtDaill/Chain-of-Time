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
    public override void _Ready(){

    }

    public override void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        
        //Displays the detail of the current attack in some GUI element

        //If the ability cares about lanes/ranks: set positionIndex to a certain value
        //If not, set positionIndex to -1;
    }

    //Because we want to avoid having to add a special case to the BattleGUI class for opening this menu, we instead have the previous menu call this command on the targeting menu for setting up the current target instead
    public void SetAbilityForTargeting(PlayerAbility newAbility, BattleGUI parentGUI){
        //Set all variables/flags related to what ability we're currently targetting.
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
        switch(currentAbility.GetTargetingLogic()){
            //All of these Targeting Types require no input from the player.
            case TargetingLogic.Self: case TargetingLogic.MyRank: case TargetingLogic.MyLane: case TargetingLogic.AllAllies: case TargetingLogic.AllEnemies:
                break;

            case TargetingLogic.SingleTargetAlly : case TargetingLogic.SingleTargetEnemy :
                HandleSelectCharacter(input, character, caller, parentGUI);
                break;
            case TargetingLogic.AnyLaneHitsAllies: case TargetingLogic.AnyLaneHitsEnemies :
                HandleSelectLane(input, character, caller, parentGUI);
                break;
            case TargetingLogic.PlayerRank: case TargetingLogic.EnemyRank:
                HandleSelectRank(input, character, caller, parentGUI);
                break;
        }
        if(Input.IsActionJustPressed("ui_accept")){
            List<Combatant> legalTargets = GetLegalTargets(selectedTargets, caller, positionIndex);
            if(legalTargets.Count() == 0){ RejectSelection(); return null; }
            currentAbility.SetTargets(legalTargets.ToArray());
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
        if(currentAbility.GetTargetingLogic() == TargetingLogic.SingleTargetAlly && newSelection is EnemyCombatant){ RejectSelection(); return; }
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
        selectedTargets = caller.GetRoster().GetCombatantsByLane((BattleLane)positionIndex, (currentAbility.GetTargetingLogic() == TargetingLogic.AnyLaneHitsAllies), (currentAbility.GetTargetingLogic() == TargetingLogic.AnyLaneHitsEnemies)).ToList();
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
        selectedTargets = caller.GetRoster().GetCombatntsByRank((BattleRank)positionIndex).ToList();
        SetPointers(caller, selectedTargets);
    }   

    //We do EVERY check as to whether or not someone is available for targeting: THIS DOES NOT INCLUDE CHECKS ABOUT THE SOURCE CHARACTER
    //Returns true if there are legal targets
    public List<Combatant> GetLegalTargets(List<Combatant> desiredTargets, Battle battle, int positionIndex = -1){
        List<Combatant> actualTargets = new List<Combatant>();
        foreach(Combatant target in desiredTargets){
            switch(currentAbility.GetTargetingLogic()){

                case TargetingLogic.SingleTargetAlly : case TargetingLogic.SingleTargetEnemy :
                    //Check Range
                    if(!currentAbility.GetEnabledPositions().Contains(target.GetPosition().GetRank())) continue;
                    actualTargets.Add(target);
                    break;

                case TargetingLogic.MyLane :
                    //Checks targets are in the correct lane
                    if(target.GetPosition().GetLane() != source.GetPosition().GetLane()) continue;
                    actualTargets.Add(target);
                    break;

                case TargetingLogic.MyRank :
                    //Checks targets are in the correct rank
                    if(target.GetPosition().GetRank() != source.GetPosition().GetRank()) continue;
                    actualTargets.Add(target);
                    break;

                case TargetingLogic.AnyLaneHitsAllies : case TargetingLogic.AnyLaneHitsEnemies :
                    if(positionIndex == -1) throw new ArgumentException("Must define a positionIndex for an AnyLane ability");
                    //Checks targets are in the correct lane
                    if((int)target.GetPosition().GetLane() != positionIndex) continue;
                    actualTargets.Add(target);
                    break;

                case TargetingLogic.PlayerRank :
                case TargetingLogic.EnemyRank :
                    if(positionIndex == -1) throw new ArgumentException("Must define a positionIndex for AnyRank ability");
                    //Checks targets are in the correct rank
                    if((int)target.GetPosition().GetLane() != positionIndex) continue;
                    actualTargets.Add(target);
                    break;

                case TargetingLogic.AllEnemies :
                    actualTargets = desiredTargets;
                    //Make checks for certain taget types.
                    return actualTargets;

                case TargetingLogic.Self :
                case TargetingLogic.AllAllies :
                    //You can always target your allies or yourself, no check nessicary.
                    return desiredTargets;
            }
        }
        return actualTargets;
    }

    //Perfroms any of the tasks that corispond with an ability being rejected for targeting
    public void RejectSelection(){

    }
    public void SetPointers(Battle battle, Combatant character){
        SetPointers(battle, new List<Combatant>(){character});
    }
    public void SetPointers(Battle battle, List<Combatant> characters = null){
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