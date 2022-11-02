using System;
using Godot;
using static PMBattleUtilities;
public class SwapMenu : BattleMenu{
    PMCharacter subject, rightTarget, leftTarget;

    private bool rightSelected = false;
    public override PMPlayerAbility HandleInput(MenuInput input, PMPlayerCharacter character, PMBattle caller)
    {
        switch(input){
            case MenuInput.Right :
                if(!rightSelected && rightTarget != null){
                    rightSelected = false;
                    leftTarget.GetNode<Sprite3D>("Pointer").Visible = true;
                    rightTarget.GetNode<Sprite3D>("Pointer").Visible = false;
                }
                break;
            case MenuInput.Left :
                if(rightSelected && leftTarget != null){
                    rightSelected = true;
                    rightTarget.GetNode<Sprite3D>("Pointer").Visible = true;
                    leftTarget.GetNode<Sprite3D>("Pointer").Visible = false;
                }
                break;
            case MenuInput.Select :
                if(rightSelected){
                    rightTarget.GetNode<Sprite3D>("Pointer").Visible = false;
                    caller.StartPositionSwap(subject.myPosition, rightTarget.myPosition);
                }else{
                    leftTarget.GetNode<Sprite3D>("Pointer").Visible = false;
                    caller.StartPositionSwap(subject.myPosition, leftTarget.myPosition);
                }
                parentGUI.ExitWithoutQueueingAbility(character);
                break;
            case MenuInput.Back :
                parentGUI.ChangeMenu(1, character, caller); //Goes to Party Menu
                break;
        }
        return null;
    }

    public override void OnOpen(PMPlayerCharacter character, PMBattle caller){
        subject = character;
        switch(character.myPosition){
            case(BattlePos.HeroOne) :
                rightTarget = caller.PositionLookup(BattlePos.HeroTwo);
                leftTarget = caller.PositionLookup(BattlePos.HeroThree);
                break;
            case(BattlePos.HeroTwo) :
                rightTarget = caller.PositionLookup(BattlePos.HeroOne);
                leftTarget = caller.PositionLookup(BattlePos.HeroThree);
                break;
            case(BattlePos.HeroThree) :
                rightTarget = caller.PositionLookup(BattlePos.HeroOne);
                leftTarget = caller.PositionLookup(BattlePos.HeroTwo);
                break;
        }
        if(rightTarget != null){
            rightSelected = true;
            rightTarget.GetNode<Sprite3D>("Pointer").Visible = true;
        }else if(leftTarget != null){
            rightSelected = false;
            leftTarget.GetNode<Sprite3D>("Pointer").Visible = true;
        }else{
            throw new NotImplementedException(); //TODO Custom Exception
        }
    }
}