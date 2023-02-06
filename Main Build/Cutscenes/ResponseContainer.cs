using Godot;
using System;
using System.Collections.Generic;

public partial class ResponseContainer : VBoxContainer
{
	List<RichTextLabel> responseLabels;
	Response[] responseObjects;

	int selectedResponse = 0;

	int initialInputDelay;

	bool responding = false;

	[Export]
	private CutsceneDirector director;
	
	[Signal]
	public delegate void FinishedRespondingEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		responseLabels = new List<RichTextLabel>();
		foreach(Node child in GetChildren()){
			if(child is RichTextLabel) responseLabels.Add((RichTextLabel)child);
		}
		responseObjects = new Response[responseLabels.Count];
		ClearLabels();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(responding && initialInputDelay == 0){
			for(int i = 0; i < responseLabels.Count; i++){
				responseLabels[i].GetChild<ColorRect>(0).Visible = (i == selectedResponse);
			}
			if(Input.IsActionJustPressed("ui_up")){
				if(selectedResponse > 0){
					selectedResponse--;
				}
			}else if(Input.IsActionJustPressed("ui_down")){
				if(selectedResponse < responseObjects.Length - 1){
					selectedResponse++;
				}
			}else if(Input.IsActionJustPressed("ui_accept")){
				director.MoveToNewExchange(responseObjects[selectedResponse].GetNextExchangeIndex());
				this.responding = false;
				ClearLabels();
			}
		}
		if(initialInputDelay > 0) initialInputDelay--;
	}

	public void DisplayResponses(Response[] responses){
		ClearLabels();
		
		responseObjects = responses;
		for(int i = 0; i < responses.Length; i++){
			if(i > responseLabels.Count) break;
			responseLabels[i].Text = responses[i].GetText();
		}
		responding = true;
		selectedResponse = 0;
		initialInputDelay = 20; //TODO: Make this more professional
	}

	public void ClearLabels(){
		foreach(RichTextLabel label in responseLabels){
			label.Text = "";
		}
		for(int i = 0; i < responseLabels.Count; i++){
				responseLabels[i].GetChild<ColorRect>(0).Visible = false;
		}
	}
}
