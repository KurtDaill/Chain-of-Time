using Godot;
using System;
using System.Collections.Generic;
//using DialogueManagerRuntime;
public partial class CutsceneDirector : Node3D
{
	[Export(PropertyHint.File)]
	public string filepath;
	public override void _Ready(){
		FileAccess file = FileAccess.Open(filepath, FileAccess.ModeFlags.Read);
		List<string>lines = new List<string>();
		while(file.GetPosition() < file.GetLength()){
			string temp = file.GetLine();
			GD.Print(temp);
			lines.Add(temp);
		}
		ScreenPlayLoader.Load(lines.ToArray());
	}
	/*
	[Export(PropertyHint.File)]
	private string dialogueFilePath = "res://Cutscenes/Dialogue Files";
	[Export]
	private Godot.Collections.Array<string> castNames;
	private Dictionary<string, Actor> cast;
	private AnimationPlayer animPlay;
	private Resource dialogue;

	[Export]
	NodePath balloonPath;

	CharacterBallon balloon;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		dialogue = GD.Load<Resource>(dialogueFilePath);
		cast = new Dictionary<string, Actor>();
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
		foreach(string name in castNames){
			var temp = GetTree().Root.FindChild(name, true);
			if(temp!= null && temp is Actor){
				cast.Add(name, (Actor)temp);
			}
		}
		balloon = GetNode<CharacterBallon>(balloonPath);
		//await ToSignal(balloon, "ready");
		//StartDialogue();
	}

	public void StartDialogue(){
		//balloon.Start(GD.Load<Resource>(dialogueFilePath), "start", new Godot.Collections.Array<Variant>());
	}

	public async void playAnimation(string animation){
		animPlay.Play(animation);
		await ToSignal(animPlay, "animation_finished");
	}
	*/
}
