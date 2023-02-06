using Godot;
using System;
using System.Collections.Generic;
//using DialogueManagerRuntime;
public partial class CutsceneDirector : Node3D
{
	[Export(PropertyHint.File)]
	public string filepath;
	[Export]
	public DialogueLabel dLabel;
	[Export]
	public string dialogueNextAction;
	[Export]
	public ResponseContainer resContainer;

	ScreenPlay play;
	private Exchange currentExchange;

	private bool waitingOnResponse = false;
	public override void _Ready(){
		FileAccess file = FileAccess.Open(filepath, FileAccess.ModeFlags.Read);
		List<string>lines = new List<string>();
		while(file.GetPosition() < file.GetLength()){
			string temp = file.GetLine();
			//GD.Print(temp);
			lines.Add(temp);
		}
		play = ScreenPlayLoader.Load(lines.ToArray());
		currentExchange = play.Start();
		DisplayLine(currentExchange.GetNextLine());
	}

    public override void _Process(double delta)
    {
		if(waitingOnResponse) return;

        if(Input.IsActionJustPressed(dialogueNextAction)){
			DisplayLine(currentExchange.GetNextLine());
		}
    }

	public void MoveToNewExchange(int newIndex){
		if(play.TryGetExchange(newIndex, out Exchange newExchange)){
			currentExchange = newExchange;
			waitingOnResponse = false;
		}
		else throw new NotImplementedException(); //TODO Custom Exception: No exchange with given index
		DisplayLine(currentExchange.GetNextLine());
	}

	public void DisplayLine(Line line){
		switch(line.GetText()){
			case "{OPT}" :
				//we hand off control to the response object
				resContainer.DisplayResponses(((ResponseLine)line).GetResponses());
				waitingOnResponse = true;
				break; 
			case "[ANIM]" :
				//Play the animation, wait for signal from animation player
				DisplayLine(currentExchange.GetNextLine()); //Temp code, ignores animation lines
				break;
			case "[SET/MOD]" :
				//Mod the value in story state
				if(!GetNode<GameMaster>("/root/GameMaster").GetStoryState().HandleModifier(line.GetModifier()))
					throw new NotImplementedException(); //TODO Custom exception, we have a mod that doesn't reference any of these values
				DisplayLine(currentExchange.GetNextLine()); //We should have something after a Mod Value to display or process
				break;
			default : //This should just be a normal line 
				dLabel.DisplayNewLine(line);
				break;
		}
	}
}
