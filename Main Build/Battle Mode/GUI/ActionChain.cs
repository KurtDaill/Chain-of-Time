using Godot;
using System;
using System.Collections.Generic;

public partial class ActionChain : Control
{
	private TextureRect[] actions;
	private TextureRect[] characterPortraits;
	private TextureRect[] chainGraphics;

	private TextureRect[] pointerGraphics;

	private bool inFlexAbilityMode = false;

	private int currentLink;

	public override void _Ready(){
		actions = new TextureRect[3];

		actions[0] = GetNode<TextureRect>("Action Link 1");
		actions[1] = GetNode<TextureRect>("Action Link 2");
		actions[2] = GetNode<TextureRect>("Action Link 3");

		chainGraphics = new TextureRect[2];
		chainGraphics[0] = GetNode<TextureRect>("Chain Graphic 1");
		chainGraphics[1] = GetNode<TextureRect>("Chain Graphic 2");

		characterPortraits = new TextureRect[3];
		characterPortraits[0] = actions[0].GetNode<TextureRect>("Character Portrait");
		characterPortraits[1] = actions[1].GetNode<TextureRect>("Character Portrait");
		characterPortraits[2] = actions[2].GetNode<TextureRect>("Character Portrait");

		pointerGraphics = new TextureRect[3];
		pointerGraphics[0] = actions[0].GetNode<TextureRect>("Pointer");
		pointerGraphics[1] = actions[1].GetNode<TextureRect>("Pointer");
		pointerGraphics[2] = actions[2].GetNode<TextureRect>("Pointer");
	}
	/*
		Ready

		Reset Function (Called when GUI Resets)
			Sets Highlight at first character, as told by GUI
			Sets Message for Characters that are unable to act
			Sets Portraits
	*/
	public void ResetActionChain(PlayerCombatant[] players){
		for(int i = 0; i < 2; i++){
			actions[i].Visible = false;
			pointerGraphics[i].Visible = false;
			if(i < chainGraphics.Length) chainGraphics[i].Visible = false;
		}
		for(int i = 0; i < players.Length; i++){
			actions[i].GetNode<Label>("Label").Text = "???";
			characterPortraits[i].Texture = players[i].GetPortrait();
			actions[i].Visible = true;
			if(i == 1) chainGraphics[0].Visible = true;
			if(i == 2) chainGraphics[1].Visible = true;
		}
		currentLink = 0;
		pointerGraphics[currentLink].Visible = true;
	}

	/*
		Log Ability (Called from GUI when the player enters an ability)
			Sets name for ability
			Sets New Highlights 
	*/
	public void LogAbility(string abilityName, bool lastAbility){
		actions[currentLink].GetNode<Label>("Label").Text = abilityName;
		pointerGraphics[currentLink].Visible = false;
		currentLink ++;
		if(!lastAbility){
			pointerGraphics[currentLink].Visible = true;
		}
	}

	public void StepBack(){
		pointerGraphics[currentLink].Visible = false;
		currentLink--;
		pointerGraphics[currentLink].Visible = true;
		actions[currentLink].GetNode<Label>("Label").Text = "???";
	}
}
