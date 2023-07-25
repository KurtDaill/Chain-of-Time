using System;
using static CutsceneUtils;
public class TextEffect{
    private int startIndex, stopIndex;
    TextEffectType type;

    public TextEffect(int start, int stop, TextEffectType type){
        startIndex = start;
        stopIndex = stop;
        this.type = type;
    }
}

public static class TextEffectUtilities{
    
}