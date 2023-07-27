using System;
using static CutsceneUtils;
public class CutsceneTextEffect{
    int startIndex, endIndex;
    TextEffectType type;
    public CutsceneTextEffect(int start, int end, string typeString){
        startIndex = start;
        endIndex = end;
        switch(typeString){
            case "b":
                type = TextEffectType.Bold;
                break;
            case "i":
                type = TextEffectType.Italics;
                break;
            case "u":
                type = TextEffectType.Underline;
                break;
            case "wave":
                type = TextEffectType.Wave;
                break;
            case "impact":
                type = TextEffectType.Impact;
                break;
            case "bigImpact":
                type = TextEffectType.BigImpact;
                break;
            case "shiver":
                type = TextEffectType.Shiver;
                break;
        }
    }
    public int getStart(){ return startIndex; }
    public int getEnd(){ return endIndex; }
}