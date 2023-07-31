using Godot;
using System;

[Tool]
public partial class CutsceneDialogueBox : Node3D
{
	Label3D characterNameLabel;
	Label3D dialogueLabel;
	Actor speaker;
	[Export]
	double textDisplaySpeed;
	double timeElapsed;
	int textCharactersDisplayed;
	string currentText;
	int[] textEffectStarts;
	int[] textEffectEnds;
	CutsceneTextEffect[] effects;
	bool printingOut = false;

	[Signal]
	public delegate void DisplayFinishedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		characterNameLabel = GetNode<Label3D>("Character Name Label");
		dialogueLabel = GetNode<Label3D>("Dialogue Label");
		try{speaker = this.GetParent<Actor>();}
		catch(InvalidCastException){throw new UnexpectedParentException("Dialogue Boxes have to be direct children of their actors!");}
		characterNameLabel.Text = speaker.GetActorName();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(printingOut){
			timeElapsed += delta;
			textCharactersDisplayed = (int)Mathf.Round(timeElapsed * textDisplaySpeed);
			if(textCharactersDisplayed > currentText.Length){
				textCharactersDisplayed = currentText.Length;
				printingOut = false;
				EmitSignal(CutsceneDialogueBox.SignalName.DisplayFinished);
			}
			dialogueLabel.Text = currentText.Substring(0, textCharactersDisplayed);
		}
	}

	public void BeginDialogue(CutsceneLine line){
		textCharactersDisplayed = 0;
		timeElapsed = 0;
		printingOut = true;
		this.Visible = true;

		currentText = line.GetText();
		effects = line.GetTextEffects();
		textEffectStarts = line.GetEffectStarts();
		textEffectEnds = line.GetEfffectEnds();
	}

	public void RushDialogue(){
		textCharactersDisplayed = currentText.Length;
		printingOut = false;
		EmitSignal(CutsceneDialogueBox.SignalName.DisplayFinished);
		dialogueLabel.Text = currentText.Substring(0, textCharactersDisplayed);
	}

	public void CloseDialogue(){
		this.Visible = false;
	}

	public bool IsDisplayingDialogue(){
		return printingOut;
	}
}
