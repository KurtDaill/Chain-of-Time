using Godot;
using System;
using System.Collections.Generic;

public partial class ResponseContainer : VBoxContainer
{
	List<RichTextLabel> responseLabels;
	Response[] responseObjects;

	bool[] selectability;

	int selectedResponse = 0;

	int initialInputDelay;

	public bool responding = false;

	[Export]
	private CutsceneDirector director;

	private AnimationPlayer animPlay;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		responseLabels = new List<RichTextLabel>();
		foreach(Node child in GetChildren()){
			if(child is RichTextLabel) responseLabels.Add((RichTextLabel)child);
		}
		responseObjects = new Response[responseLabels.Count];
		Clear();
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
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
				if(selectability[selectedResponse] == false) return;
				if(responseObjects[selectedResponse].isEnd()) director.ExitCutscene(responseObjects[selectedResponse].DoesBattleBegin());
				else{
					director.MoveToNewExchange(responseObjects[selectedResponse].GetNextExchangeIndex());
					
					//this.responding = false;
					//Clear();
				}
			}
		}
		if(initialInputDelay > 0) initialInputDelay--;
	}

	public void DisplayResponses(Response[] responses){
		Clear();
		responseObjects = responses;
		selectability = new bool[responses.Length];
		for(int i = 0; i < responses.Length; i++){
			if(i > responseLabels.Count) break;
			responseLabels[i].Text = responses[i].GetText();
			if(responses[i].GetCondition() != null){
				if(GetNode<GameMaster>("/root/GameMaster").GetStoryState().TryGetValue(responses[i].GetCondition().GetKey(), out int value)){
					if(responses[i].GetCondition().isConditionMet(value)){
						selectability[i] = true;
						continue;
					}else{
						selectability[i] = false;
						responseLabels[i].Text = "[color=red]" + responseLabels[i].Text + "[/color]";
						continue;
					}
				}
			}else{
				selectability[i] = true;
			}
		}
		responding = true;
		selectedResponse = 0;
		initialInputDelay = 20; //TODO: Make this more professional
		animPlay.Play("Open");
	}

	public void Clear(){
		foreach(RichTextLabel label in responseLabels){
			label.Text = "";
		}
		for(int i = 0; i < responseLabels.Count; i++){
				responseLabels[i].GetChild<ColorRect>(0).Visible = false;
		}
		this.Scale = new Vector2(0,1); //Resets the scale such that the open animation doesn't stutter
	}
}
