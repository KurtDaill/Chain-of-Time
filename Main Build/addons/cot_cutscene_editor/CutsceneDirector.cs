using Godot;
using System;
using System.Collections.Generic;
using static BattleMenu; //TODO Move MenuInput to a more general solution?
[Tool]
public partial class CutsceneDirector : Node3D
{
    public bool enabled = true;
    bool waitingOnAnimation = false;
    bool waitingOnTextDisplay = false;
    Dictionary<string, Actor> cast;
    Dictionary<StringName, Actor> watchedAnimations;
    ScreenPlay play;
    CutsceneBlock block;
    CutsceneAction currentAction;
    [Export(PropertyHint.File)]
    string screenPlayXMLFilePath;

    public override void _Ready()
    {
        base._Ready();
        cast = new Dictionary<string, Actor>();
        watchedAnimations = new Dictionary<StringName, Actor>();
        foreach(Node node in this.GetChildren()){
            if(node is Actor) cast.Add(((Actor)node).GetActorName(), (Actor)node);
        }
        play = ScreenPlayLoader.LoadScript(screenPlayXMLFilePath);
    }

    public override void _Process(double delta){
        HandleInput();
    }

    private void HandleInput(){
        if(Input.IsActionJustPressed("ui_select")){
            switch(currentAction.GetType().Name){
                case "CutsceneLine" :
                    /* If no concurrent Animation : 
                            If we're at the end of the text box : go to the next dialogue
                            If we're still displaying the text box : display the entire textbox
                        If concurrent Animation :
                            Wait until the animation is done, after that run the above logic
                    */
                    CutsceneLine line = (CutsceneLine) currentAction;
                    if(line.HasConcurrentAnimation() && waitingOnAnimation){
                        return;
                    }else{
                        if(waitingOnTextDisplay){
                            cast.TryGetValue(((CutsceneLine)currentAction).GetSpeaker(), out Actor actor);
                            actor.GetDialogueBox().RushDialogue();
                        }else{
                            //Go to the next dialogue
                        }
                    }
                    break;
            }
        }
    }

    public void PlayCutscene(){
        block = play.Start();
        waitingOnAnimation = false;
        StartAction(block.StartBlockAndPeekFirstAction());
        currentAction = block.GetNextAction(); //Should Get the first action loaded
    }

    public void AdvanceToNextAction(){
        currentAction = block.GetNextAction();
        StartAction(currentAction);
    }

    public void EndBlock(){
        
    }

    public void StartAction(CutsceneAction act){
        switch(act.GetType().Name){
            case "CutsceneLine" :
                CutsceneLine line = (CutsceneLine) block.GetNextAction();
                cast.TryGetValue(line.GetSpeaker(), out Actor currentActor);
                cast.TryGetValue(line.GetSpeaker(), out Actor actor);
                if(actor == null) throw new ArgumentException("No actor found that maches speaker: " + line.GetSpeaker() + " for dialogue line: " + line.GetText());
                
                currentActor.GetDialogueBox().DisplayFinished += OnTextDisplayComplete;

                if(line.HasConcurrentAnimation()){
                    currentActor.GetAnimationPlayer().AnimationFinished += OnCutsceneAnimationComplete;
                    watchedAnimations.Add(line.GetConcurrentAnimationName(), currentActor);
                }
                
                actor.SpeakLine(line);
                break;
        }

    }

    public void OnCutsceneAnimationComplete(StringName animation){
        waitingOnAnimation = false;
        watchedAnimations.TryGetValue(animation, out Actor actor);
        actor.GetAnimationPlayer().AnimationFinished -= OnCutsceneAnimationComplete;
    }

    public void OnTextDisplayComplete(){
        waitingOnTextDisplay = false;
        cast.TryGetValue(((CutsceneLine)currentAction).GetSpeaker(), out Actor actor);
        actor.GetDialogueBox().DisplayFinished -= OnTextDisplayComplete;
    }
}

public static class CutsceneUtils{
	public enum TextEffectType{
        Bold,
        Italics,
        Underline,
        Wave,
        Impact,
        BigImpact,
        Shiver
    }
}
