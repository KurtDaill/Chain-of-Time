using System;
using Godot;
using static BattleUtilities;

public partial class SwapMenu : BattleMenu{
    private int targetLane, targetRank;
    public override void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        targetLane = (int) character.GetPosition().GetLane();
        targetRank = (int) character.GetPosition().GetRank();
        if(targetRank < 2) targetRank++;
        else{
            targetRank = 0;
            targetLane = 0;
        }
        caller.GetRoster().ShowPointer();
    }

    public override PlayerAbility HandleInput(PlayerInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        switch(input){
            case PlayerInput.Right :
                if(targetRank == 2){ Reject(); return null;}
                targetRank++;
                break;
            case PlayerInput.Left :
                if(targetRank == 0){ Reject(); return null;}
                targetRank--;
                break;
            case PlayerInput.Up :
                if(targetLane == 2){ Reject(); return null;}
                targetLane++;
                break;
            case PlayerInput.Down :
                if(targetLane == 0){ Reject(); return null;}
                targetLane--;
                break;
            case PlayerInput.Back :
                caller.GetRoster().HidePointer();
                parentGUI.ChangeMenu(0, character);
                break;
            case PlayerInput.Select :
                if(caller.GetRoster().GetCombatant(new BattlePosition((BattleLane)targetLane, (BattleRank)targetRank)) == character){ //Checks to make sure the player isn't swapping a character with themselves
                    Reject();
                    return null;
                }
                caller.GetRoster().HidePointer();
                
                return character.SetupAndGetSwap(caller.GetRoster(), new BattlePosition((BattleLane)targetLane, (BattleRank)targetRank));
        }
        //If there was any input, make sure there's not an empty space in front of our target Rank and we make sure the pointer is in the right place.
        if(input != PlayerInput.None){
            while(3 - caller.GetRoster().GetCombatantsByLane((BattleLane)targetLane, true, false).Length > targetRank && targetRank < 2){
                targetRank++;
            }
            caller.GetRoster().SetPointerPosition(targetLane, targetRank);
        }
        return null;
    }

    private void Reject(){

    }
}
