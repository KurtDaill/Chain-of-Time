using Godot;
using System;
using static GameplayUtilities;

public partial class TutorialNode : Node
{
    [Export]
    private bool debugDisabled;
    [Export]
    public Godot.Collections.Array<string> tutorialMessages;
    [Export]
    public StringName tutorialTriggerName;

    private GameMaster gm;
    private TutorialTextBox box;
    private Timer inputDelay;

    public override void _Ready(){
        gm = this.GetNode<GameMaster>("/root/GameMaster"); 
        box = this.GetNode<TutorialTextBox>("/root/TutorialTextBox");
        inputDelay = new();
        inputDelay.OneShot = true;
        this.AddChild(inputDelay);
        ProcessMode = ProcessModeEnum.WhenPaused;
    }

    public override void _Process(double delta){
        if(HandleInput(gm.ReadInputRemotely())){
            GetTree().Paused = false;
        }
    }

    public void Trigger(){
        if(debugDisabled) return;
        //Checks whether we should activate!
        if(gm.TutorialTriggerCheckIn(tutorialTriggerName)){
            GetTree().Paused = true;
            box.StartDisplayMessages(tutorialMessages);
        }else{
            GD.Print("Tutorial Message Bypassed: " + tutorialTriggerName);
        }
    }

    //Returns true when we're done
    public bool HandleInput(PlayerInput input){
        if(inputDelay.TimeLeft != 0) return false;
        if(input == PlayerInput.Select){
            inputDelay.Start(0.75);
            return box.AdvanceTextBoxAndCheckForEnd();
        }
        return false;
    }
}
