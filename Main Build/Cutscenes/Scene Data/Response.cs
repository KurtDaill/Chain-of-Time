using System;

public class Response{
    private string text;
    private ResponseCondition condition;
    private int nextExchange = -1;

    private bool endsDialogue;

    private bool triggersCombat;

    public Response(string text, ResponseCondition condition, int nextExchange, bool endsDialogue = true, bool triggersCombat = true){
        this.text = text;
        this.condition = condition;
        this.nextExchange = nextExchange;
        this.endsDialogue = endsDialogue;
        this.triggersCombat = triggersCombat;
    }

    public string GetText(){
        return text;
    }

    public int GetNextExchangeIndex(){
        return nextExchange;
    }

    public bool isEnd(){
        return endsDialogue;
    }

    public bool DoesBattleBegin(){
        return triggersCombat;
    }

    public ResponseCondition GetCondition(){
		return condition;
	}
}