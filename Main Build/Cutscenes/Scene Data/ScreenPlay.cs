using System;

public class ScreenPlay{
    Exchange[] exchanges;

    public ScreenPlay(Exchange[] exchanges){
        this.exchanges = exchanges;
    }

    public Exchange Start(){
        return exchanges[0];
    }

    public bool TryGetExchange(int index, out Exchange newExchange){
        if(exchanges.Length > index){
            newExchange = exchanges[index];
            return true;
        }
        newExchange = null;
        return false;
    }
}