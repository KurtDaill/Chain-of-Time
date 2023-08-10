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
    [Export]
    CutsceneDialogueBox dialogueBox;
    [Export]
    bool autoPlay;
    Dictionary<string, CutsceneShot> shotList;
    Dictionary<string, Marker3D> blockingMarks;
    string playerCharacterName;

    StoryState storyState;
    CutsceneCamera mainCutsceneCamera;
    CutsceneResponseBox playerCharacterResponseBox;
    AnimationPlayer animPlay;
    //Implements the momento pattern
    Stack<PackedScene> cutsceneStateHistory;

    public override void _Ready()
    {
        ConfigureChildNodes();
        base._Ready();
        watchedAnimations = new Dictionary<StringName, Actor>();
        watchedBlockingMoves = new Dictionary<string, Actor>();
        play = ScreenPlayLoader.LoadScript(screenPlayXMLFilePath);
        storyState = GetTree().Root.GetNode<GameMaster>("GameMaster").GetStoryState();
        playerCharacterName = playerCharacter.GetActorName();


        //Set initial camera position;
        if(shotList.TryGetValue(initialShotName, out CutsceneShot initialShotObject))mainCutsceneCamera.StartTransition(initialShotObject.GetShotDetails(), "cut");
        else throw new ArgumentException("Initial shot: " + initialShotName + " not found in this cutscene");
        

        cutsceneStateHistory = new Stack<PackedScene>();
        if(autoPlay)PlayCutscene();
    }

    //Because we deleted and reload our children during editing, we want to have this set as its own function to be called then instead of just in ready
    private void ConfigureChildNodes(){
        cast = new Dictionary<string, Actor>();
        this.GetChildren();
        beatTimer = this.GetNode<Timer>("BeatTimer");
        beatTimer.Timeout += AdvanceToNextAction;
        foreach(Node node in this.GetChildren()){
            if(node is Actor) cast.Add(((Actor)node).GetActorName(), (Actor)node);
        }
        shotList = new Dictionary<string, CutsceneShot>();
        foreach(CutsceneShot shot in this.GetNode("Shot List").GetChildren()){
            shotList.Add(shot.Name, shot);
        }
        blockingMarks = new Dictionary<string, Marker3D>(); 
        foreach(Marker3D blockingMark in this.GetNode<Node3D>("Blocking Marks").GetChildren()){
            blockingMarks.Add(blockingMark.Name, blockingMark);
        }
        mainCutsceneCamera = this.GetNode<CutsceneCamera>("CameraModifierHandle/Cutscene Camera");
        animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
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
                        if(dialogueBox.IsDisplayingDialogue()){
                            dialogueBox.RushDialogue();
                        }else{
                            dialogueBox.CloseDialogue();
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
        /*
        if(Input.IsActionJustPressed("debug_4")){
            LoadLiveState(0);
        }*/
    }

    public void PlayCutscene(){
        block = play.Start();
        waitingOnAnimation = false;
        currentAction = block.StartBlockAndGetFirstAction();
        StartAction(currentAction);
        //SaveLiveState();
        //currentAction = block.GetNextAction(); //Should Get the first action loaded
    }

    public void AdvanceToNextAction(){
        currentAction = block.GetNextAction();
        StartAction(currentAction);
    }

    public async void StartAction(CutsceneAction act){
        //SaveLiveState();
        switch(act.GetType().Name){
            case "CutsceneLine" :
                CutsceneLine line = (CutsceneLine) act;
                cast.TryGetValue(line.GetSpeaker(), out Actor lineActor);
                if(lineActor == null && line.GetSpeaker() != "???") throw new ArgumentException("No actor found that maches speaker: " + line.GetSpeaker() + " for dialogue line: " + line.GetText());
                //TODO Handle Unknown (???) speakers better
                if(line.HasConcurrentAnimation()){
                    if(cast.TryGetValue(line.GetConcurrentAnimation().GetCharacter(), out Actor concurrentAnimationCharacter)){
                        concurrentAnimationCharacter.GetAnimationPlayer().AnimationFinished += OnCutsceneAnimationComplete;
                        watchedAnimations.Add(line.GetConcurrentAnimation().GetAnimation(), concurrentAnimationCharacter);
                    }else{
                        throw new NotImplementedException(); //TODO Custom Exception
                    }
                    if(!concurrentAnimationCharacter.GetAnimationPlayer().HasAnimation(line.GetConcurrentAnimation().GetAnimation()))
                        throw new NotImplementedException("Animation: " + line.GetConcurrentAnimation().GetAnimation() + " not found on Actor: " + concurrentAnimationCharacter.GetActorName());
                    concurrentAnimationCharacter.GetAnimationPlayer().Play(line.GetConcurrentAnimation().GetAnimation());
                    waitingOnAnimation = true;
                }
                //lineActor.SpeakLine(line);
                dialogueBox.BeginDialogue(line);

                break;
            case "CutsceneCharacterAnimation":
                CutsceneCharacterAnimation anim = (CutsceneCharacterAnimation) act;
                cast.TryGetValue(anim.GetCharacter(), out Actor animActor);
                if(!animActor.GetAnimationPlayer().HasAnimation(anim.GetAnimation())) throw new NotImplementedException(); //TODO: Custom Exception
                animActor.GetAnimationPlayer().Play(anim.GetAnimation());
                if(!anim.IsIdleLoop()){
                    animActor.GetAnimationPlayer().AnimationFinished += OnCutsceneAnimationComplete;
                    watchedAnimations.Add(anim.GetAnimation(), animActor);
                }else{
                    AdvanceToNextAction();
                }
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
            case "CutsceneEnvironmentAnimation":
                CutsceneEnvironmentAnimation envAnimation = (CutsceneEnvironmentAnimation) act;
                this.animPlay.Play(envAnimation.GetAnimation());
                await ToSignal(this.animPlay, AnimationPlayer.SignalName.AnimationFinished);
                AdvanceToNextAction();
                return;
            case "CutsceneSmashCut": //TODO Add in some kind of system to track whether we're waiting on anything or should advance to the next
                CutsceneSmashCut smashCut = (CutsceneSmashCut) act;
                foreach(CutsceneCharacterMove smashCutMove in smashCut.GetCharacterMoves()){
                    cast.TryGetValue(smashCutMove.GetCharacterName(), out Actor actor);
                    blockingMarks.TryGetValue(smashCutMove.GetBlockingMarkerName(), out Marker3D marker);
                    actor.GlobalPosition = marker.GlobalPosition;
                }
                foreach(CutsceneCharacterAnimation smashCutSetAnimation in smashCut.GetCharacterAnimations()){
                    cast.TryGetValue(smashCutSetAnimation.GetCharacter(), out Actor actor);
                    if(!actor.GetAnimationPlayer().HasAnimation(smashCutSetAnimation.GetAnimation())) throw new NotImplementedException();
                    if(!smashCutSetAnimation.IsIdleLoop()){
                        actor.GetAnimationPlayer().AnimationFinished += OnCutsceneAnimationComplete;
                        watchedAnimations.Add(smashCutSetAnimation.GetAnimation(), actor);
                    }
                    actor.GetAnimationPlayer().Play(smashCutSetAnimation.GetAnimation());
                }
                foreach(string nodeName in smashCut.GetNodesToBeHidden()){
                    ((Node3D)this.FindChild(nodeName, true, true)).Visible = false;
                }
                foreach(string nodeName in smashCut.GetNodesToBeShown()){
                    ((Node3D)this.FindChild(nodeName, true, true)).Visible = true;
                }
                if(shotList.TryGetValue(smashCut.GetMyCameraMove().GetTargetShot(), out CutsceneShot smashShot)){
                    mainCutsceneCamera.StartTransition(smashShot.GetShotDetails(), smashCut.GetMyCameraMove().GetTransitionType(), smashCut.GetMyCameraMove().GetTransitionLength());
                }else throw new NotImplementedException(); //TODO Write Custom Exception
                break;
            case "CutsceneEndBlock":
                CutsceneEndBlock endBlock = (CutsceneEndBlock) act;
                if(endBlock.IsResponseBlock()){
                    playerCharacterResponseBox = dialogueBox.StartDialogueResponse(endBlock.GetDialogueOptions());
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
        //TODO Figure out a better way of determining whether or not this animaiton is the hold up
        if(currentAction.GetType().Name == "CutsceneCharacterAnimation" || currentAction.GetType().Name == "CutsceneSmashCut") AdvanceToNextAction(); //We only progress to the next block if this animation is the thing holding up the show
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
    
    /*
    //Implements the "Save State" section of the momento pattern using packed scenes
    public void SaveLiveState(){
        PackedScene momento = new PackedScene();
        momento.Pack(this);
        cutsceneStateHistory.Push(momento);
    }

    //Implements the "Restore State" sectino of the moment pattern using packed scenes;
    public async void LoadLiveState(int depth){
        for(int i = 0; i < depth; i++){
            cutsceneStateHistory.Pop();
        }
        PackedScene previousCutscenePacked = cutsceneStateHistory.Pop();
        CutsceneDirector previousCutscene = previousCutscenePacked.Instantiate<CutsceneDirector>();
        Godot.Collections.Array<Node> myChildren = this.GetChildren();
        for(int i = 0; i < previousCutscene.GetChildren().Count; i++){
            myChildren[i].Free();
            this.AddChild(previousCutscene.GetChild(i));
        }
        //await ToSignal(this.GetChild(0), Node.SignalName.Ready);
        this.currentAction = block.GoBackwardsInBlock(depth);
        ConfigureChildNodes();
        StartAction(currentAction);
        //this.GetParent().AddChild(previousCutscene);
        //this.GetParent().GetChild<CutsceneDirector>(2).GetNode<CutsceneCamera>("Cutscene Camera").Current = true;
        //this.Free();
    }
    */   
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
