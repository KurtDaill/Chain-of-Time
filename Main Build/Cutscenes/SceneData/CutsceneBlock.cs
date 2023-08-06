using System;
using System.Collections.Generic;
using System.Linq;
public class CutsceneBlock{
    string name;
    CutsceneAction[] actions;
    //Basically represents "what I'm currently looking at' and points to the last thing we returned;
    int head = 0;

    public CutsceneBlock(string name, CutsceneAction[] actions){
        this.name = name;
        this.actions = actions;
    }

    public string GetName(){
        return name;
    }

    public CutsceneAction StartBlockAndPeekFirstAction(){
        head = -1;
        return actions[0];
    }

    public CutsceneAction StartBlockAndGetFirstAction(){
        head = 0;
        return actions[head];
    }

    public CutsceneAction GetNextAction(){
        head++;
        return actions[head];
    }

    public CutsceneAction GoBackwardsInBlock(int depth){
        head -= depth;
        return actions[head];
    }

    public CutsceneAction PeekNextAction(){
        return actions[(head+1)];
    }

    public bool HasNextAction(){
        return head < actions.Length+1;
    }
}