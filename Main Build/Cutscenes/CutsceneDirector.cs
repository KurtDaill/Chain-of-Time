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
	[Export]
	public string playerCharacterActor = "Cato";
	[Export]
	public ExplorePlayer sceneExplorePlayer;
	ScreenPlay play;
	private Exchange currentExchange;

	private Line currentLine;

	AnimationPlayer animPlay;

	Dictionary<string, Actor> cast; 

	private bool waiting = false;
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
		currentLine = currentExchange.GetNextLine();
		//DisplayLine(currentLine);
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
		cast = new Dictionary<string, Actor>();
		foreach(Node node in this.GetChildren()){
			if(node is Actor){
				Actor child = (Actor) node;
				cast.Add(child.GetActorName(), child);
			}
		}
	}

    public override void _Process(double delta)
    {
		if(waiting) return;

        if(Input.IsActionJustPressed(dialogueNextAction)){
			if(currentLine.isEnd()){
				ExitCutscene();
			}
			if(currentLine.GetGotoIndex() != -1){
				MoveToNewExchange(currentLine.GetGotoIndex());
			}else{
				currentLine = currentExchange.GetNextLine();
				DisplayLine(currentLine);
			}
		}
    }

	public void MoveToNewExchange(int newIndex){
		if(play.TryGetExchange(newIndex, out Exchange newExchange)){
			currentExchange = newExchange;
			waiting = false;
		}
		else throw new NotImplementedException(); //TODO Custom Exception: No exchange with given index
		currentLine = currentExchange.GetNextLine();
		DisplayLine(currentLine);
	}

	public async void DisplayLine(Line line){
		switch(line.GetText()){
			case "{OPT}" :
				//we hand off control to the response object
				resContainer.DisplayResponses(((ResponseLine)line).GetResponses());
				waiting = true;
				break; 
			case "[ANIM]" :
				//Play the animation, wait for signal from animation player
					//currentLine = currentExchange.GetNextLine();
					//DisplayLine(currentLine); //Temp code, ignores animation lines
					animPlay.Play(line.GetAnimation());
					dLabel.ClearLine();
					waiting = true;
					await ToSignal(animPlay, "animation_finished");
					waiting = false;
					currentLine = currentExchange.GetNextLine();
					DisplayLine(currentLine);
				break;
			case "[SET/MOD]" :
				//Mod the value in story state
				if(!GetNode<GameMaster>("/root/GameMaster").GetStoryState().HandleModifier(line.GetModifier()))
					throw new NotImplementedException(); //TODO Custom exception, we have a mod that doesn't reference any of these values
					currentLine = currentExchange.GetNextLine();
					DisplayLine(currentLine); //We should have something after a Mod Value to display or process
				break;
			default : //This should just be a normal line 
				dLabel.DisplayNewLine(line);
				SetDialogueBalloons(line.GetSpeaker());
				break;
		}
	}

	public void StartDialogue(){
		DisplayLine(currentLine);
	}

	public void StartCutscene(){
		sceneExplorePlayer.SetPlayerControl(false);
		sceneExplorePlayer.Visible = false;
	}

	public void ExitCutscene(){
		GD.Print("Exit Cutscene");
		resContainer.Clear();
		dLabel.ClearLine();
		GetNode<ExplorePlayer>("/root/Node3D/ExplorePlayer").SetPlayerControl(true);
		GetNode<ExplorePlayer>("/root/Node3D/ExplorePlayer").Visible = true;
		cast.TryGetValue(playerCharacterActor, out Actor temp);
		temp.SetVisiblity(false);
	}

	public void SetDialogueBalloons(string speaker){
		foreach(Actor actor in cast.Values){
			actor.HideBalloon();
			if(actor.GetActorName() == speaker){
				actor.ShowBalloon();
			}
		}
	}
}
