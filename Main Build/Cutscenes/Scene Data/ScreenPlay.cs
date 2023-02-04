using System;

public class ScreenPlay{
    Exchange[] exchanges;
    int currentIndex;

    public ScreenPlay(Exchange[] exchanges){
        this.exchanges = exchanges;
        currentIndex = 0;
    }

    public Exchange Start(){
        currentIndex = 0;
        return exchanges[currentIndex];
    }
}