using Godot;
using System;
using System.Collections.Generic;

public partial class DialogueLabel : RichTextLabel
{
	[Export]
	public RichTextLabel speakerLabel;

	[Export]
	public int charactersPerSecond;

	private double displayTime;

	private double timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		displayTime = 1/charactersPerSecond;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Handle Inputs
		timer += delta;
		if(timer > displayTime){
			timer -= displayTime;
			if(this.VisibleCharacters < this.Text.Length) this.VisibleCharacters++; 
		}
	}

	public void DisplayNewLine(Line newLine){
		this.Text = newLine.GetText();
		this.VisibleCharacters = 0;
		this.speakerLabel.Text = " " + newLine.GetSpeaker(); //Added a space to fit the text box better
	}

	public void ClearLine(){
		this.Text = "";
		this.speakerLabel.Text = "";
	}
}
