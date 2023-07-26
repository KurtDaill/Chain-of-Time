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
	private AudioStreamPlayer altVoice;

	[Export]
	private int voiceLetterRatio;
	private int voiceTimer = 0;

	private TextureRect button;

	private double timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		displayTime = 1/charactersPerSecond;
		button = this.GetNode<TextureRect>("Button");
		button.Visible = false;
		//ClearLine();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(this.Text != ""){
			//Handle Inputs
			if(timer >= displayTime){
				timer -= displayTime;
				if(this.VisibleCharacters < this.Text.Length){
					this.VisibleCharacters++;
					if(this.Text[VisibleCharacters - 1] == '.' || this.Text[VisibleCharacters - 1] == '?'|| this.Text[VisibleCharacters - 1] == '!'){ //if this letter is a period
						timer -= 0.25; //we wait longer
						voiceTimer = voiceLetterRatio;
					}else if(this.Text[VisibleCharacters - 1] == ','|| this.Text[VisibleCharacters - 1] == ';'){ //if this letter is a comma/semi-colon
						timer -= 0.12; //we wait longer
					}

					if(voiceTimer >= voiceLetterRatio){
						
						if(altVoice != null && (this.speakerLabel.Text == " ???" || this.speakerLabel.Text == " Sejanus?"|| this.speakerLabel.Text == " Death")) altVoice.Play();
						else voice.Play();
						voiceTimer = 0;
					}else{
						voiceTimer++;
					}
				}
			}else{
				timer += delta;
			}
		}
	}

	/*public void DisplayNewLine(Line newLine){
		this.Text = newLine.GetText();
		this.VisibleCharacters = 0;
		this.speakerLabel.Text = " " + newLine.GetSpeaker(); //Added a space to fit the text box better
		button.Visible = true;
		timer = 0;
	}

	public void ClearLine(){
		this.Text = "";
		this.speakerLabel.Text = "";
		button.Visible = false;
	}

	public bool IsDoneDisplaying(){
		return !(this.VisibleCharacters < this.Text.Length);
	}

	public void DisplayAll(){
		this.VisibleCharacters = this.Text.Length;
	}*/
}
