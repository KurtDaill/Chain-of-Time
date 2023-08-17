using Godot;
using System;

[Tool]
public partial class CutsceneDialogueBox : Control
{
	[Export]
	RichTextLabel dialogueLabel;
	[Export]
	Control myResponseBox;
	int titleLength;
	CutsceneResponseBox responses;
	bool waitingForNextWord = true;
	Actor speaker;
	double delayTimer = 0;

	//Actor speaker;
	[Export]
	double textDisplaySpeed = 20;
	double timeElapsed;
	bool printingOut = false;

	[Signal]
	public delegate void DisplayFinishedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//characterNameLabel = GetNode<Label3D>("Character Name Label");
		//dialogueLabel = GetNode<Label3D>("Dialogue Label");
		responses = (CutsceneResponseBox) myResponseBox;
		//try{speaker = this.GetParent<Actor>();}
		//catch(InvalidCastException){throw new UnexpectedParentException("Dialogue Boxes have to be direct children of their actors!");}
		dialogueLabel.Text = "";
		//responses.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(printingOut){
			if(delayTimer > 0){
				delayTimer -= delta;
				return;
			}
			timeElapsed += delta;
			dialogueLabel.VisibleCharacters = titleLength + (int)Mathf.Round(timeElapsed * textDisplaySpeed);
			if(dialogueLabel.VisibleCharacters >= dialogueLabel.GetParsedText().Length){
				printingOut = false;
				EmitSignal(CutsceneDialogueBox.SignalName.DisplayFinished);
			}
			switch(dialogueLabel.GetParsedText()[dialogueLabel.VisibleCharacters]){
				case ' ': waitingForNextWord = true; break;
				case '.': case ':' : case ';': case '?': case '!' : //add a long delay
					dialogueLabel.VisibleCharacters ++;
					delayTimer = 0.05;
					break;
				case ',' : //add a short delay
					dialogueLabel.VisibleCharacters ++;
					delayTimer = 0.025;
					break;
				default:
					if(waitingForNextWord){
						string nextWord = "";
					//Get all of the text for the next word
						if(dialogueLabel.GetParsedText()[dialogueLabel.VisibleCharacters..].IndexOf(' ') == -1){
							//This block is only reach if the current word is the last word
							nextWord = dialogueLabel.GetParsedText()[dialogueLabel.VisibleCharacters..];
						}else{
							nextWord = dialogueLabel.GetParsedText().Substring(dialogueLabel.VisibleCharacters, 
							dialogueLabel.GetParsedText()[dialogueLabel.VisibleCharacters..].IndexOf(' '));
						}
						if(nextWord.Length > 7){
							speaker.PlayTwoSyllablePip();
						}else{
							speaker.PlayOneSyllablePip();
						}
						waitingForNextWord = false;
					} break;
			}
		}
	}

	public void BeginDialogue(CutsceneLine line, Actor speaker){
		timeElapsed = 0;
		printingOut = true;
		this.Visible = true;
		this.speaker = speaker;
		dialogueLabel.Text = "[color=" + speaker.GetColor().ToHtml() +"][b]" + line.GetSpeaker() + "- [/b][/color] ";
		titleLength = line.GetSpeaker().Length + 1;
		dialogueLabel.VisibleCharacters = titleLength;
		dialogueLabel.Text += line.GetText();
		waitingForNextWord = true;
	}

	public void RushDialogue(){
		dialogueLabel.VisibleCharacters = -1;
		printingOut = false;
		EmitSignal(CutsceneDialogueBox.SignalName.DisplayFinished);
	}

	public void CloseDialogue(){
		this.Visible = false;
		//dialogueLabel.Text = "";
	}

	public bool IsDisplayingDialogue(){
		return printingOut;
	}

	public CutsceneResponseBox StartDialogueResponse(CutsceneDialogueResponse[] responseObjects){
		this.Visible = true;
    	responses.PopulateResponsesAndShow(responseObjects);
		return responses;
	}
}
