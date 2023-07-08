using System;
using Godot;
using static BattleUtilities;

public partial class SwapMenu : BattleMenu{
    PlayerCombatant subject, rightTarget, leftTarget;

    private bool rightSelected = false;
    public override PlayerAbility HandleInput(MenuInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI)
    {
        switch(input){
            case MenuInput.Right :
                if(!rightSelected && rightTarget != null){
                    rightSelected = false;
                    leftTarget.SetTargetGUIElements(true);
                    rightTarget.SetTargetGUIElements(false);
                }
                break;
            case MenuInput.Left :
                if(rightSelected && leftTarget != null){
                    rightSelected = true;
                    rightTarget.SetTargetGUIElements(true);
                    leftTarget.SetTargetGUIElements(false);
                }
                break;
            case MenuInput.Select :
                if(rightSelected){
                    rightTarget.SetTargetGUIElements(false);
                    //Queue the "SwapPositions" Ability
                    return subject.SetupAndGetSwap(caller.GetRoster(), rightTarget);
                }else{
                    leftTarget.SetTargetGUIElements(false);
                    return subject.SetupAndGetSwap(caller.GetRoster(), leftTarget);
                    //caller.GetRoster().SwapCharacters(subject.GetPosition(), leftTarget.GetPosition());
                }
                //parentGUI.ExitWithoutQueueingAbility(character);
                //break;
            case MenuInput.Back :
                if(leftTarget != null) leftTarget.SetTargetGUIElements(false);
                if(rightTarget != null) rightTarget.SetTargetGUIElements(false);
                parentGUI.ChangeMenu(1, character); //Goes to Party Menu
                break;
        }
        return null;
    }

    public override void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        subject = character;
        switch(character.GetPosition()){
            case(BattlePosition.HeroFront) :
                rightTarget = (PlayerCombatant)caller.GetRoster().GetCombatant(BattlePosition.HeroMid);
                leftTarget = (PlayerCombatant)caller.GetRoster().GetCombatant(BattlePosition.HeroBack);
                break;
            case(BattlePosition.HeroMid) :
                rightTarget = (PlayerCombatant)caller.GetRoster().GetCombatant(BattlePosition.HeroFront);
                leftTarget = (PlayerCombatant)caller.GetRoster().GetCombatant(BattlePosition.HeroBack);
                break;
            case(BattlePosition.HeroBack) :
                rightTarget = (PlayerCombatant)caller.GetRoster().GetCombatant(BattlePosition.HeroFront);
                leftTarget = (PlayerCombatant)caller.GetRoster().GetCombatant(BattlePosition.HeroMid);
                break;
        }
        if(rightTarget != null){
            rightSelected = true;
            rightTarget.SetTargetGUIElements(true);
            //rightTarget.GetNode<Sprite3D>("Pointer").Visible = true;
        }else if(leftTarget != null){
            rightSelected = false;
            leftTarget.SetTargetGUIElements(true);
            //leftTarget.GetNode<Sprite3D>("Pointer").Visible = true;
        }else{
            throw new NotImplementedException(); //TODO Custom Exception
        }
    }
}
