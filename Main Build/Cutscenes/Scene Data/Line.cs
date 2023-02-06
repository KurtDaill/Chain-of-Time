using System;

public class Line{
    protected string text;
    protected string speaker;
    protected TextEffect[] effects;
    protected ScreenplayModifier mod;
    protected string animation; 
    protected int gotoIndex;

    public Line(string text, string speaker, TextEffect[] effects, ScreenplayModifier mod, string animation, int gotoIndex = -1, Response[] responses = null){
        this.text = text;
        this.speaker = speaker;
        this.effects = effects;
        this.mod = mod;
        this.animation = animation;
        this.gotoIndex = gotoIndex;
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
}