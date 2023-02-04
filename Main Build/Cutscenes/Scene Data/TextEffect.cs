using System;
using static TextEffectUtilities;
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
    public enum TextEffectType{
        Wave,
        Shake,
        BigShake,
        Bounce
    }
}