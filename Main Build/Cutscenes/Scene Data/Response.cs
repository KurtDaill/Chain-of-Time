using System;

public class Response{
    private string text;
    private ResponseCondition condition;
    private int nextExchange = -1;

    private bool endsDialogue;

    public Response(string text,ResponseCondition condition, int nextExchange, bool endsDialogue = true){
        this.text = text;
        this.condition = condition;
        this.nextExchange = nextExchange;
        this.endsDialogue = endsDialogue;
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
}