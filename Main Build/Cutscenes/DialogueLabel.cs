using Godot;
using System;
using System.Collections.Generic;

public partial class DialogueLabel : RichTextLabel
{
	[Export]
	public RichTextLabel speakerLabel;

	[Export]
	public double charactersPerSecond;

	private double displayTime;

	[Export]
	private AudioStreamPlayer voice;

	[Export]
	private int voiceLetterRatio;
	private int voiceTimer = 0;

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
		if(timer >= displayTime){
			timer -= displayTime;
			if(this.VisibleCharacters < this.Text.Length){
				this.VisibleCharacters++;
				if(this.Text[VisibleCharacters - 1] == '.' || this.Text[VisibleCharacters - 1] == '?'|| this.Text[VisibleCharacters - 1] == '!'){ //if this letter is a period
					timer -= 0.5; //we wait longer
					voiceTimer = voiceLetterRatio;
				}else if(this.Text[VisibleCharacters - 1] == ','|| this.Text[VisibleCharacters - 1] == ';'){ //if this letter is a comma/semi-colon
					timer -= 0.25; //we wait longer
				}

				if(voiceTimer >= voiceLetterRatio){
					voice.Play();
					voiceTimer = 0;
				}else{
					voiceTimer++;
				}
			}
		}else{
			timer += delta;
		}
	}

	public void DisplayNewLine(Line newLine){
		this.Text = newLine.GetText();
		this.VisibleCharacters = 0;
		this.speakerLabel.Text = " " + newLine.GetSpeaker(); //Added a space to fit the text box better
		timer = 0;
	}

	public void ClearLine(){
		this.Text = "";
		this.speakerLabel.Text = "";
	}
}
