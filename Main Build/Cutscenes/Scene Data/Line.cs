using System;

public class Line{
    private string text;
    private string speaker;
    private TextEffect[] effects;
    private ScreenplayModifier mods;
    private string animation;
    private int gotoIndex;

    private Response[] responses;

    public Line(string text, string speaker, TextEffect[] effects, ScreenplayModifier mods, string animation, int gotoIndex = -1, Response[] responses = null){
        this.text = text;
        this.speaker = speaker;
        this.effects = effects;
        this.mods = mods;
        this.animation = animation;
        this.gotoIndex = gotoIndex;
        this.responses = responses;
    }

    public string GetText(){
        return text;
    }
    public string GetSpeaker(){
        return speaker;
    }

    public bool GetResponses(out Response[] responses){
            responses = this.responses;
            return (this.responses != null);
    }
}