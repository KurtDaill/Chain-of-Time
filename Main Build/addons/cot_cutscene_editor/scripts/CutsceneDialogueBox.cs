using Godot;
using System;

[Tool]
public partial class CutsceneDialogueBox : Node3D
{
	[Export]
	Label characterNameLabel;
	[Export]
	RichTextLabel dialogueLabel;
	Actor speaker;
	[Export]
	double textDisplaySpeed;
	double timeElapsed;
	int[] textEffectStarts;
	int[] textEffectEnds;
	CutsceneTextEffect[] effects;
	bool printingOut = false;

	[Signal]
	public delegate void DisplayFinishedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//characterNameLabel = GetNode<Label3D>("Character Name Label");
		//dialogueLabel = GetNode<Label3D>("Dialogue Label");
		try{speaker = this.GetParent<Actor>();}
		catch(InvalidCastException){throw new UnexpectedParentException("Dialogue Boxes have to be direct children of their actors!");}
		characterNameLabel.Text = speaker.GetActorName();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(printingOut){
			timeElapsed += delta;
			dialogueLabel.VisibleCharacters = (int)Mathf.Round(timeElapsed * textDisplaySpeed);
			if(dialogueLabel.VisibleCharacters >= dialogueLabel.Text.Length){
				printingOut = false;
				EmitSignal(CutsceneDialogueBox.SignalName.DisplayFinished);
			}
		}
	}

	public void BeginDialogue(CutsceneLine line){
		dialogueLabel.VisibleCharacters = 0;
		timeElapsed = 0;
		printingOut = true;
		this.Visible = true;

		dialogueLabel.Text = line.GetText();
		effects = line.GetTextEffects();
		textEffectStarts = line.GetEffectStarts();
		textEffectEnds = line.GetEfffectEnds();
	}

	public void RushDialogue(){
		dialogueLabel.VisibleCharacters = -1;
		printingOut = false;
		EmitSignal(CutsceneDialogueBox.SignalName.DisplayFinished);
	}

	public void CloseDialogue(){
		this.Visible = false;
		dialogueLabel.Text = "";
	}

	public bool IsDisplayingDialogue(){
		return printingOut;
	}
}
