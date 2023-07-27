using System;
using System.Collections.Generic;
using System.Linq;
public class CutsceneBlock{
    string name;
    CutsceneAction[] actions;
    Queue<CutsceneAction>actionQueue;

    public CutsceneBlock(string name, CutsceneAction[] actions){
        this.name = name;
        this.actions = actions;
    }

    public string GetName(){
        return name;
    }

    public CutsceneAction StartBlockAndPeekFirstAction(){
        actionQueue = new Queue<CutsceneAction>(actions);
        return actionQueue.Peek();
    }

    public CutsceneAction GetNextAction(){
        return actionQueue.Dequeue();
    }

    public CutsceneAction PeekNextAction(){
        return actionQueue.Peek();
    }

    public bool HasNextAction(){
        return actionQueue.TryPeek(out CutsceneAction temp);
    }
}