using System;
using Godot;
using System.Collections.Generic;

public abstract class CutsceneAction{

}

public class CutsceneLine : CutsceneAction{
    string speaker, text;
    CutsceneTextEffect[]effects;
    int[] effectStartIndexes;
    int[] effectEndIndexes;
    bool hasConcurrentAnimation;
    CutsceneCharacterAnimation concurrentAnimation;

    public CutsceneLine(string speaker, string text, CutsceneTextEffect[] effects, bool hasAnim = false, CutsceneCharacterAnimation concurrentAnim = null){
        this.speaker = speaker;
        this.text = text;
        this.effects = effects;

        effectStartIndexes = new int[effects.Length];
        effectEndIndexes = new int[effects.Length];
        for(int i = 0; i < effects.Length; i++){
            effectStartIndexes[i] = effects[i].getStart();
            effectEndIndexes[i] = effects[i].getEnd();
        }
        hasConcurrentAnimation = hasAnim;
        concurrentAnimation = concurrentAnim;
    }

    public string GetText(){return text;}
    public string GetSpeaker(){return speaker;}
    public int[] GetEffectStarts(){return effectStartIndexes;}
    public int[] GetEfffectEnds(){return effectEndIndexes;}
    public CutsceneTextEffect[] GetTextEffects(){return effects;}
    public bool HasConcurrentAnimation(){return hasConcurrentAnimation;}
    public CutsceneCharacterAnimation GetConcurrentAnimation(){return concurrentAnimation;}
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
    bool idleLoop;
    public CutsceneCharacterAnimation(string character, string animation, bool concurent, bool idleLoop){
        this.character = character;
        this.animation = animation;
        this.concurent = concurent;
        this.idleLoop = idleLoop;
    }
    public string GetAnimation(){return animation;}
    public string GetCharacter(){return character;}
    public bool IsIdleLoop(){return idleLoop;}
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
    public string GetFlagName(){return name;}
    public bool GetFlagValue(){return value;}
}

public class CutsceneModStoryValue : CutsceneAction{
    string name;
    int modifier;
    public CutsceneModStoryValue(string name, int modifier){
        this.name = name;
        this.modifier = modifier;
    }
    public string GetValueName(){return name;}
    public int GetValueMod(){return modifier;}
}

public class CutsceneSetStoryValue : CutsceneAction{
    string name;
    int set;
    public CutsceneSetStoryValue(string name, int set){
        this.name = name;
        this.set = set;
    }
    public string GetValueName(){return name;}
    public int GetValueSet(){return set;}
}

public class CutsceneEndBlock : CutsceneAction{
    CutsceneDialogueResponse[] options;
    CutsceneGoToBlock gotoBlockStatement;

    public CutsceneEndBlock(CutsceneDialogueResponse[] options){
        this.options = options;
        gotoBlockStatement = null;
    }
    public CutsceneEndBlock(CutsceneGoToBlock gotoBlockStatement){
        this.options = null;
        this.gotoBlockStatement = gotoBlockStatement;
    }
    public bool IsResponseBlock(){
        return gotoBlockStatement == null;
    }
    public string GetGotoBlockTarget(){
        return gotoBlockStatement.GetTargetBlock();
    }
    public CutsceneDialogueResponse[] GetDialogueOptions(){
        return options;
    }
}

public class CutsceneGoToBlock : CutsceneAction{
    protected string targetBlock = "INVALID";
    public CutsceneGoToBlock(string target){
        targetBlock = target;
    }
    public CutsceneGoToBlock(){}
    public string GetTargetBlock(){
        return targetBlock;
    }
}

public class CutsceneDialogueResponse : CutsceneGoToBlock{
    string text;
    public CutsceneDialogueResponse(string text, string target){
        this.text = text;
        this.targetBlock = target;
    }
    public string GetResponseText(){
        return text;
    }
}

public class CutsceneCameraMove : CutsceneAction{
    string targetShot;
    string transitionType;
    double transitionLength;
    public CutsceneCameraMove(string targetShot, string transitionType, double transitionLength){
        this.targetShot = targetShot;
        this.transitionType = transitionType;
        this.transitionLength = transitionLength;
    }

    public string GetTargetShot(){return targetShot;}
    public string GetTransitionType(){return transitionType;}
    public double GetTransitionLength(){return transitionLength;}
}

public class CutsceneCharacterMove : CutsceneAction{
    string characterName;
    string blockingMarkerName;
    public CutsceneCharacterMove(string characterName, string blockingMarkerName){
        this.characterName = characterName;
        this.blockingMarkerName = blockingMarkerName;
    }
    public string GetCharacterName(){return characterName;}
    public string GetBlockingMarkerName(){return blockingMarkerName;}
}

public class CutsceneEnvironmentAnimation : CutsceneAction{
    string animation;
    public CutsceneEnvironmentAnimation(string animation){
        this.animation = animation;
    }
    public string GetAnimation(){return animation;}
}

public class CutsceneSmashCut : CutsceneAction{
    List<CutsceneCharacterMove> characterMoves;
    List<CutsceneCharacterAnimation> characterAnimations;
    List<string> nodesToBeShown;
    List<string> nodesToBeHidden;
    public CutsceneSmashCut(List<CutsceneCharacterMove> characterMoves, List<CutsceneCharacterAnimation> characterAnimations, List<string> nodesToBeShown, List<string> nodesToBeHidden){
        this.characterMoves = characterMoves;
        this.characterAnimations = characterAnimations;
        this.nodesToBeShown = nodesToBeShown;
        this.nodesToBeHidden = nodesToBeHidden;    
    }
    public List<CutsceneCharacterMove> GetCharacterMoves(){return characterMoves;}
    public List<CutsceneCharacterAnimation> GetCharacterAnimations(){return characterAnimations;}
    public List<string> GetNodesToBeShown(){return nodesToBeShown;}
    public List<string> GetNodesToBeHidden(){return nodesToBeHidden;}
}