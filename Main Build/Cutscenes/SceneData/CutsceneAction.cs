using System;

public class CutsceneAction{

}

public class CutsceneLine : CutsceneAction{
    string speaker, text;
    CutsceneTextEffect[]effects;
    int[] effectStartIndexes;
    int[] effectEndIndexes;

    public CutsceneLine(string speaker, string text, CutsceneTextEffect[] effects){
        this.speaker = speaker;
        this.text = text;
        this.effects = effects;

        effectStartIndexes = new int[effects.Length];
        effectEndIndexes = new int[effects.Length];
        for(int i = 0; i < effects.Length; i++){
            effectStartIndexes[i] = effects[i].getStart();
            effectEndIndexes[i] = effects[i].getStart();
        }
    }
}

public class CutsceneAsyncAnimation : CutsceneAction{
    string animation;
    public CutsceneAsyncAnimation(string animation){
        this.animation = animation;
    }
    public string GetAnimation(){return animation;}
}

public class CutsceneCharacterAnimation : CutsceneAction{
    string character;
    string animation;
    bool concurent;
    public CutsceneCharacterAnimation(string character, string animation, bool concurent){
        this.character = character;
        this.animation = animation;
        this.concurent = concurent;
    }
    public string GetAnimation(){return animation;}
    public string GetCharacter(){return character;}
}

public class CutsceneBeat : CutsceneAction{
    double delay;
    public CutsceneBeat(double newDelay){
        delay = newDelay;
    }
    public double GetDelay(){return delay;}
}

public class CutsceneSetStoryFlag : CutsceneAction{
    string name;
    bool value;
    public CutsceneSetStoryFlag(string name, bool value){
        this.name = name;
        this.value = value;
    }
}