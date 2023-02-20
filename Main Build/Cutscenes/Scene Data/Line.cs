using System;

public class Line{
    protected string text;
    protected string speaker;
    protected TextEffect[] effects;
    protected ScreenplayModifier mod;
    protected string animation; 
    protected int gotoIndex;

    bool endLine;

    public Line(string text, string speaker, TextEffect[] effects, ScreenplayModifier mod, string animation, int gotoIndex = -1, Response[] responses = null, bool endLine = false){
        this.text = text;
        this.speaker = speaker;
        this.effects = effects;
        this.mod = mod;
        this.animation = animation;
        this.gotoIndex = gotoIndex;
        this.endLine = endLine;
    }

    public Line(){}

    public string GetText(){
        return text;
    }
    public string GetSpeaker(){
        return speaker;
    }

    public ScreenplayModifier GetModifier(){
        return mod;
    }

    public int GetGotoIndex(){
        return gotoIndex;
    }

    public string GetAnimation(){
        return animation;
    }

    public bool isEnd(){
        return endLine;
    }
}