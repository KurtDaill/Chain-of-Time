using System;
using System.Collections.Generic;
using System.Linq;
public class CutsceneBlock{
    string name;
    CutsceneAction[] actions;
    Queue<CutsceneAction>actionQueue;

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