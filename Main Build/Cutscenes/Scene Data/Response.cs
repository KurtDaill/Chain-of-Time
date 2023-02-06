using System;

public class Response{
    private string text;
    private ResponseCondition condition;
    private int nextExchange = -1;

    public Response(string text,ResponseCondition condition, int nextExchange){
        this.text = text;
        this.condition = condition;
        this.nextExchange = nextExchange;
    }

    public string GetText(){
        return text;
    }

    public int GetNextExchangeIndex(){
        return nextExchange;
    }
}