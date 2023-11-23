using Godot;
using System;

public partial class TutorialTextBox : Control
{
    protected RichTextLabel mainTextBox;
    protected Godot.Collections.Array<string> tutorialMessages;
    protected int currentMessageIndex;
    [Export]
    protected double displaySpeed = 1;
    protected double displayCount = 0;

    public override void _Ready(){
        mainTextBox = this.GetNode<RichTextLabel>("TextBoxGraphics/MainTextBox");
        tutorialMessages = new();
        this.Visible = false;
        ProcessMode = ProcessModeEnum.WhenPaused;
    }

    public override void _Process(double delta){
        if(this.Visible == false) return;
        if(tutorialMessages.Count > 0){
            if(mainTextBox.VisibleCharacters < mainTextBox.Text.Length){
                displayCount += delta * displaySpeed;
                mainTextBox.VisibleCharacters = (int) Math.Round(displayCount);
            }
        }
    }

    public void StartDisplayMessages(Godot.Collections.Array<string> messages){
        tutorialMessages = messages;
        currentMessageIndex = 0;
        ShowNewMessage(currentMessageIndex);
        this.Visible = true;
    }

    //returns true if the text box has no further messages to show
    public bool AdvanceTextBoxAndCheckForEnd(){
        //Checks if we're still slowly displaying characters
        if(mainTextBox.VisibleCharacters < mainTextBox.Text.Length){
            mainTextBox.VisibleCharacters = mainTextBox.Text.Length;
            return false;
        }else{
            //Checks if there's any more messages we have to display
            if(currentMessageIndex + 1 < tutorialMessages.Count){
                currentMessageIndex ++;
                ShowNewMessage(currentMessageIndex);
                return false;
            }else{
                //We've shown every message, and send back true
                this.Visible = false;
                return true;
            }
        }
    }

    private void ShowNewMessage(int index){
        mainTextBox.Text = tutorialMessages[index];
        mainTextBox.VisibleCharacters = 0;
        displayCount = 0;
    }
}
