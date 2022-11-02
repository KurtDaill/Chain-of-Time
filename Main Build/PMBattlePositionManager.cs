using Godot;
using System;
using static PMBattleUtilities;
public class PMBattlePositionManager : Spatial
{
    uint currentSwap;
    AnimationPlayer animPlay;
    Transform transR, transL, cTrans1, cTrans2, cTrans3;
    Spatial posNodeR, posNodeL, cNode1, cNode2, cNode3;
    PMCharacter charR, charL, cChar1, cChar2, cChar3;
    BattlePos originalPositionR, originalPositionL, cOPos1, cOPos2, cOPos3;

    //Used to handle whether we're in the special case where slots 2 & 3 have to movce forward at the same time: a "crunch"
    bool crunch;

    public override void _Ready(){
        animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
    }

    /*
        Start Position Swap begins swapping postitions of two player character.
        It triggers an animation based on the given positions, and at the end of those
        animations, the "CompletePositionSwap" function is called using the "Call Method" track
    */ 
    public void StartPositionSwap(BattlePos PositionOne, BattlePos PositionTwo){ //TODO Implement Enemy position swaping
        currentSwap = (uint)PositionOne | (uint)PositionTwo;
        switch(currentSwap){
            case 0b_011000:
                animPlay.Play("SwapH12");
                posNodeR = this.GetNode<Spatial>("Hero 1");
                posNodeL = this.GetNode<Spatial>("Hero 2");
                break;
            case 0b_101000:
                animPlay.Play("SwapH13");
                posNodeR = this.GetNode<Spatial>("Hero 1");
                posNodeL = this.GetNode<Spatial>("Hero 3");
                break;
            case 0b_110000:
                animPlay.Play("SwapH23");
                posNodeR = this.GetNode<Spatial>("Hero 2");
                posNodeL = this.GetNode<Spatial>("Hero 3");
                break;
            case 0b_000110:
                animPlay.Play("SwapE12");
                posNodeR = this.GetNode<Spatial>("Enemy 1");
                posNodeL = this.GetNode<Spatial>("Enemy 2");
                break;
            case 0b_000101:
                animPlay.Play("SwapE13");
                posNodeR = this.GetNode<Spatial>("Enemy 1");
                posNodeL = this.GetNode<Spatial>("Enemy 3");
                break;
            case 0b_000011:
                animPlay.Play("SwapE23");
                posNodeR = this.GetNode<Spatial>("Enemy 2");
                posNodeL = this.GetNode<Spatial>("Enemy 3");
                break;
            default:
                throw new NotImplementedException();
        }
        transR = posNodeR.Transform;
        charR = posNodeR.GetChild<PMCharacter>(0);
        originalPositionR = charR.myPosition;

        transL = posNodeL.Transform;
        charL = posNodeL.GetChild<PMCharacter>(0);
        originalPositionL = charL.myPosition;
    }

    public void StartPositionCrunch(bool hero){
        crunch = true;
        if(hero){
            animPlay.Play("SwapHCrunch");
            cNode1 = this.GetNode<Spatial>("Hero 1");
            cNode2 = this.GetNode<Spatial>("Hero 2");
            cNode3 = this.GetNode<Spatial>("Hero 3");
        }else{
            animPlay.Play("SwapECrunch");
            cNode1 = this.GetNode<Spatial>("Enemy 1");
            cNode2 = this.GetNode<Spatial>("Enemy 2");
            cNode3 = this.GetNode<Spatial>("Enemy 3");
        }
        cTrans1 = cNode1.Transform;
        cTrans2 = cNode2.Transform;
        cTrans3 = cNode3.Transform;

        cChar1 = cNode1.GetChild<PMCharacter>(0);
        cChar2 = cNode2.GetChild<PMCharacter>(0);
        cChar3 = cNode3.GetChild<PMCharacter>(0);

        if(cChar1 != null)cOPos1 = cChar1.myPosition;
        else cOPos1 = BattlePos.EnemyOne;
        cOPos2 = cChar2.myPosition;
        cOPos3 = cChar3.myPosition;
    }

    public void FinishPositionSwap(BattleRoster roster){//TODO make this less ugly
        if(crunch){
            roster.SetCharacter(cChar3, cOPos2);
            roster.SetCharacter(cChar2, cOPos1);
            if(cChar1 != null)roster.SetCharacter(cChar1, cOPos3);
            if(cChar1 != null)cNode1.RemoveChild(cChar1);
            cNode2.RemoveChild(cChar2);
            cNode3.RemoveChild(cChar3);
            cNode1.Transform = cTrans1;
            cNode2.Transform = cTrans2;
            cNode3.Transform = cTrans3;
            cNode1.AddChild(cChar2);
            cNode2.AddChild(cChar3);
            if(cChar1 != null)cNode3.AddChild(cChar1);
        }else{
            roster.SetCharacter(charR, originalPositionL);
            roster.SetCharacter(charL, originalPositionR);
            posNodeR.RemoveChild(charR);
            posNodeL.RemoveChild(charL);
            posNodeR.Transform = transR;
            posNodeL.Transform = transL;
            posNodeR.AddChild(charL);
            posNodeL.AddChild(charR);
        }
        currentSwap = 0;
    }
}
