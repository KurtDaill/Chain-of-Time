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
    bool waitingOnBlockingMove = false;
    Dictionary<string, Actor> cast;
    Dictionary<StringName, Actor> watchedAnimations;
    Dictionary<string, Actor> watchedBlockingMoves;
    ScreenPlay play;
    CutsceneBlock block;
    CutsceneAction currentAction;
    Timer beatTimer;
    [Export(PropertyHint.File)]
    string screenPlayXMLFilePath;
    [Export]
    Actor playerCharacter;
    [Export]
    string initialShotName;
    Dictionary<string, CutsceneShot> shotList;
    Dictionary<string, Marker3D> blockingMarks;
    string playerCharacterName;

    StoryState storyState;
    CutsceneCamera mainCutsceneCamera;
    CutsceneResponseBox playerCharacterResponseBox;

    public override void _Ready()
    {
        base._Ready();
        cast = new Dictionary<string, Actor>();
        watchedAnimations = new Dictionary<StringName, Actor>();
        watchedBlockingMoves = new Dictionary<string, Actor>();
        foreach(Node node in this.GetChildren()){
            if(node is Actor) cast.Add(((Actor)node).GetActorName(), (Actor)node);
        }
        play = ScreenPlayLoader.LoadScript(screenPlayXMLFilePath);
        beatTimer = this.GetNode<Timer>("BeatTimer");
        beatTimer.Timeout += AdvanceToNextAction;
        storyState = GetTree().Root.GetNode<GameMaster>("GameMaster").GetStoryState();
        playerCharacterName = playerCharacter.GetActorName();

        shotList = new Dictionary<string, CutsceneShot>();
        foreach(CutsceneShot shot in this.GetNode("Shot List").GetChildren()){
            shotList.Add(shot.Name, shot);
        }

        blockingMarks = new Dictionary<string, Marker3D>();
        foreach(Marker3D blockingMark in this.GetNode<Node3D>("Blocking Marks").GetChildren()){
            blockingMarks.Add(blockingMark.Name, blockingMark);
        }

        mainCutsceneCamera = this.GetNode<CutsceneCamera>("Cutscene Camera");
        //Set initial camera position;
        if(shotList.TryGetValue(initialShotName, out CutsceneShot initialShotObject))mainCutsceneCamera.StartTransition(initialShotObject.GetShotDetails(), "cut");
        else throw new ArgumentException("Initial shot: " + initialShotName + " not found in this cutscene");
    }

    public override void _Process(double delta){
        HandleInput();
    }

    private void HandleInput(){
        if(Input.IsActionJustPressed("ui_accept")){
            switch(currentAction.GetType().Name){
                case "CutsceneLine" :
                    /* If no concurrent Animation : 
                            If we're at the end of the text box : go to the next dialogue
                            If we're still displaying the text box : display the entire textbox
                        If concurrent Animation :
                            Wait until the animation is done, after that run the above logic
                    */
                    CutsceneLine line = (CutsceneLine) currentAction;
                    cast.TryGetValue(((CutsceneLine)currentAction).GetSpeaker(), out Actor actor);
                    if(line.HasConcurrentAnimation() && waitingOnAnimation){
                        return;
                    }else{
                        if(actor.GetDialogueBox().IsDisplayingDialogue()){
                            actor.GetDialogueBox().RushDialogue();
                        }else{
                            actor.GetDialogueBox().CloseDialogue();
                            AdvanceToNextAction();
                        }
                    }
                    break;
                case "Beat" : //The beat delay is handled via signal
                    return;
                case "CutsceneAnimation" : //The animation is handled via signal
                    return;
                case "CutsceneCameraMove" : //Camera move completion is handled via signal
                    return;
                case "CutsceneEndBlock" : //act Has to be the response block loaded into playerCharacterResponseBox for us to reach this block of code
                    GotoNextBlock(playerCharacterResponseBox.GetFinalDestination());
                    break;
            }
        }
        else if(Input.IsActionJustPressed("ui_down")){
            switch(currentAction.GetType().Name){
                case "CutsceneEndBlock" :
                    playerCharacterResponseBox.GoDownList();
                    break;
            }
        }
        else if(Input.IsActionJustPressed("ui_up")){
            switch(currentAction.GetType().Name){
                case "CutsceneEndBlock" :
                    playerCharacterResponseBox.GoUpList();
                    break;
            }
        }
    }

    public void PlayCutscene(){
        block = play.Start();
        waitingOnAnimation = false;
        currentAction = block.StartBlockAndGetFirstAction();
        StartAction(currentAction);
        //currentAction = block.GetNextAction(); //Should Get the first action loaded
    }

    public void AdvanceToNextAction(){
        currentAction = block.GetNextAction();
        StartAction(currentAction);
    }

    public void StartAction(CutsceneAction act){
        switch(act.GetType().Name){
            case "CutsceneLine" :
                CutsceneLine line = (CutsceneLine) act;
                cast.TryGetValue(line.GetSpeaker(), out Actor lineActor);
                if(lineActor == null) throw new ArgumentException("No actor found that maches speaker: " + line.GetSpeaker() + " for dialogue line: " + line.GetText());

                if(line.HasConcurrentAnimation()){
                    lineActor.GetAnimationPlayer().AnimationFinished += OnCutsceneAnimationComplete;
                    watchedAnimations.Add(line.GetConcurrentAnimation().GetAnimation(), lineActor);
                    if(!lineActor.GetAnimationPlayer().HasAnimation(line.GetConcurrentAnimation().GetAnimation()))
                        throw new NotImplementedException("Animation: " + line.GetConcurrentAnimation().GetAnimation() + " not found on Actor: " + lineActor.GetActorName());
                    lineActor.GetAnimationPlayer().Play(line.GetConcurrentAnimation().GetAnimation());
                    waitingOnAnimation = true;
                }
                lineActor.SpeakLine(line);
                break;
            case "CutsceneCharacterAnimation":
                CutsceneCharacterAnimation anim = (CutsceneCharacterAnimation) act;
                cast.TryGetValue(anim.GetCharacter(), out Actor animActor);
                watchedAnimations.Add(anim.GetAnimation(), animActor);
                animActor.GetAnimationPlayer().Play(anim.GetAnimation());
                break;
            case "CutsceneBeat" :
                CutsceneBeat beat = (CutsceneBeat) act;
                beatTimer.WaitTime = beat.GetDelay();
                beatTimer.Start();
                break;
            case "CutsceneSetStoryFlag" :
                CutsceneSetStoryFlag flag = (CutsceneSetStoryFlag) act;
                storyState.TrySetFlag(flag.GetFlagName(), flag.GetFlagValue());
                AdvanceToNextAction();
                break;
            case "CutsceneSetStoryValue" :
                CutsceneSetStoryValue set = (CutsceneSetStoryValue) act;
                storyState.TryModValue(set.GetValueName(), set.GetValueSet());
                AdvanceToNextAction();
                break;
            case "CutsceneModStoryValue" :
                CutsceneModStoryValue mod = (CutsceneModStoryValue) act;
                storyState.TryModValue(mod.GetValueName(), mod.GetValueMod());
                AdvanceToNextAction();
                break;
            case "CutsceneCameraMove" :
                CutsceneCameraMove move = (CutsceneCameraMove) act;
                if(shotList.TryGetValue(move.GetTargetShot(), out CutsceneShot newShot)){
                    if(mainCutsceneCamera.StartTransition(newShot.GetShotDetails(), move.GetTransitionType(), move.GetTransitionLength())){
                        mainCutsceneCamera.ShotTransitionComplete += OnCutsceneCameraMoveComplete;
                        waitingOnBlockingMove = true;
                    }else{
                        AdvanceToNextAction();
                    }
                }else{
                    new ArgumentException("Initial shot: " + move.GetTargetShot() + " not found in this cutscene");
                }
                break;
            case "CutsceneCharacterMove" :
                CutsceneCharacterMove characterMove = (CutsceneCharacterMove) act;
                if(cast.TryGetValue(characterMove.GetCharacterName(), out Actor characterMoving)){
                    if(blockingMarks.TryGetValue(characterMove.GetBlockingMarkerName(), out Marker3D mark)){
                        characterMoving.StartBlockingMovement(mark.GlobalPosition, characterMove.GetBlockingMarkerName());
                        watchedBlockingMoves.Add(characterMove.GetBlockingMarkerName(), characterMoving);
                    }else{
                        throw new ArgumentException("Character: " + characterMove.GetBlockingMarkerName() + " not found in this cutscene!");
                    }
                }else{
                    throw new ArgumentException("Character: " + characterMove.GetCharacterName() + " not found in this cutscene!");
                }
                characterMoving.CompletedBlockingMovement += OnCharacterCompleteBlockingMovement;
                break;
            case "CutsceneEndBlock":
                CutsceneEndBlock endBlock = (CutsceneEndBlock) act;
                if(endBlock.IsResponseBlock()){
                    playerCharacterResponseBox = playerCharacter.GetNode<CutsceneResponseBox>("Response Box");
                    playerCharacterResponseBox.PopulateResponsesAndShow(endBlock.GetDialogueOptions());
                }else{
                    GotoNextBlock(endBlock.GetGotoBlockTarget());
                }
                break;
        }

    }

    public void GotoNextBlock(string targetBlock){
        if(targetBlock == null || targetBlock == "INVALID") throw new ArgumentException();
        if(targetBlock == "EXIT"){
            //TODO handle quitting cutscenes
            GD.Print("So long, Gay Bowser!");
        }else{
            if(play.TryGetBlock(targetBlock, out CutsceneBlock newLoadedBlock)){
                block = newLoadedBlock;
                currentAction = block.StartBlockAndGetFirstAction();
                StartAction(currentAction);
            }else{
                throw new ArgumentException("Listed Block: " + targetBlock + " not found in screenplay! Check your XML script file!");
            }
        }
    }

    public void OnCutsceneAnimationComplete(StringName animation){
        waitingOnAnimation = false;
        watchedAnimations.TryGetValue(animation, out Actor actor);
        actor.GetAnimationPlayer().AnimationFinished -= OnCutsceneAnimationComplete;
        if(currentAction.GetType().Name == "CutsceneCharacterAnimation") AdvanceToNextAction(); //We only progress to the next block if this animation is the thing holding up the show
        watchedAnimations.Remove(animation);
    }

    public void OnCutsceneCameraMoveComplete(){
        mainCutsceneCamera.ShotTransitionComplete -= OnCutsceneCameraMoveComplete;
        AdvanceToNextAction();
    }

    public void OnCharacterCompleteBlockingMovement(string markerName){
        waitingOnBlockingMove = false;
        watchedBlockingMoves.TryGetValue(markerName, out Actor actor);
        actor.CompletedBlockingMovement -= OnCharacterCompleteBlockingMovement;
        watchedBlockingMoves.Remove(markerName);
        AdvanceToNextAction();
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
