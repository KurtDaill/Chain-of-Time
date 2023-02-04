/*
using Godot;
using System;
using DialogueManagerRuntime;
public partial class CharacterBallon : CanvasLayer
{
	bool dialogueLineAutoTime = false;
	[Export]
	private RichTextLabel characterNamePlate;
	//private DialogueLabel dialogueBox;
	private Godot.Object currentLine;
	
	private bool waitingForInput;
	[Export]
	private CharacterBallonResponses responseMenu;

	private Godot.Collections.Array<Godot.Variant> tempGameStates;
	[Export(PropertyHint.File)]
	private string debugDialogueResPath;
	private Resource dialogueRes;
	[Export]
	private NodePath dialogueLabelNode;
	private Godot.Object dLabel;
	[Export]
	private Theme disabledResponse;
	[Export]
	private Theme normalResponse;
	public override void _Ready()
	{
		characterNamePlate.Visible = false;
		dLabel = GetNode(dialogueLabelNode);
		dLabel.Set("visible", false);
		responseMenu.Visible = false;
		Action actOnMutate = () => {OnMutation();};
		GetNode("/root/DialogueManager").Connect("mutation", Callable.From(actOnMutate));
		var res = GD.Load<Resource>(debugDialogueResPath);
		Start(res, "0", new Godot.Collections.Array<Variant>());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async void SetDialogueLine(Godot.Collections.Dictionary dict){
		if(dict.Count == 0){
			//Exit the Cutscene somehoww
		}
		waitingForInput = false;
		Godot.Object line = (Godot.Object) GD.Load<GDScript>("res://addons/dialogue_manager/dialogue_line.gd").New(dict);
		currentLine = line;

		if((string)line.Get("character") != ""){
			characterNamePlate.Visible = true;
			characterNamePlate.Text = (string) line.Get("character");
		}

		dLabel.Set("visible", false);
		dLabel.Set("dialogue_line", line);
		dLabel.Call("reset_height");
		await ToSignal(dLabel, "finished_resize");

		responseMenu.Visible = false;
		Godot.Collections.Array<Godot.Object> responses = (Godot.Collections.Array<Godot.Object>)line.Get("responses");
		if(responses.Count > 0){
			if(responses.Count > responseMenu.responses.Count){//if this, then we have too many responses to render
				throw new NotImplementedException(); //TODO custom exception
			}
			for(int i = 0; i < responses.Count; i++){
				 responseMenu.responses[i].Text = (string)responses[i].Get("text");
				 if(!(bool)responses[i].Get("is_allowed")){
					responseMenu.responses[i].Theme = disabledResponse;
				 }else{
					responseMenu.responses[i].Theme = normalResponse;
				 }
			}
		}

		this.Visible = true;
		dLabel.Set("Visible", true);

		if(((string)dLabel.Get("Text")).Length != 0){
			dLabel.Call("type_out");
			await ToSignal(dLabel, "finished_typing");
		}

			dialogueLineAutoTime = false;
			float? lineTime = null;
		try{//Dealing with Dialogue manager's weakly typed logic
			lineTime = (float?)line.Get("time");
		}catch(InvalidCastException){
			dialogueLineAutoTime = true;
		}
		if(((Godot.Collections.Array)line.Get("responses")).Count > 0){
			responseMenu.Visible = true;
			//Setup Response Menu
		}else if(dialogueLineAutoTime){
			await ToSignal(GetTree().CreateTimer(((string)line.Get("text")).Length * 0.02), "timeout");
			Next((string)line.Get("next_id"));
		}else if(lineTime != null){
			await ToSignal(GetTree().CreateTimer((double)lineTime), "timeout");
			Next((string)line.Get("next_id"));
		}else{
			waitingForInput = true;
		}	
	}

	public Godot.Object GetDialogueLine(){
		return currentLine;
	}

	public async void Start(Resource dialogueResource, string title, Godot.Collections.Array<Godot.Variant> gameStates){
		tempGameStates = gameStates;
		waitingForInput = false;
		dialogueRes = dialogueResource;
		SetDialogueLine(await DialogueManager.GetNextDialogueLine(dialogueRes, title, tempGameStates));
	}

	public async void Next(string next_id){
		SetDialogueLine(await DialogueManager.GetNextDialogueLine(dialogueRes, next_id, tempGameStates));
	}

	public void OnMutation(){
		waitingForInput = false;
		//TODO Hide the Ballon
	}
}
*/
