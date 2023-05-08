using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerCharacterReadout : TextureRect
{
	Label HP, SP;
	Label maxHPLabel, maxSPLabel;
	TextureRect hpIconFull, hpIconHalf, hpIconEmpty, spIconFull, spIconHalf, spIconEmpty, highLight;

	HBoxContainer statusBar;
	public PMPlayerCharacter character;

	public override void _Ready()
	{
		HP = GetNode<Label>("HP");
		SP = GetNode<Label>("SP");
		maxHPLabel = GetNode<Label>("Max HP");
		maxSPLabel = GetNode<Label>("Max SP");
		hpIconFull = this.GetNode<TextureRect>("HP Icon Full");
		hpIconHalf = this.GetNode<TextureRect>("HP Icon Half");
		hpIconEmpty = this.GetNode<TextureRect>("HP Icon Empty");
		spIconFull = this.GetNode<TextureRect>("SP Icon Full");
		spIconHalf = this.GetNode<TextureRect>("SP Icon Half");
		spIconEmpty = this.GetNode<TextureRect>("SP Icon Empty");
		highLight = this.GetNode<TextureRect>("Highlight");
		statusBar = this.GetNode<HBoxContainer>("Status Bar");
	}
	public void UpdateHP(int newHP, int newMaxHP){
		HP.Text = "" + newHP;
		maxHPLabel.Text = "" + newMaxHP;
		//Updates the little icon next to the HP number
		hpIconFull.Visible = false;
		hpIconHalf.Visible = false;
		hpIconEmpty.Visible = false;
		if(newHP <= 0){
			hpIconEmpty.Visible = true;
		}else if(newHP <= newMaxHP/2){
			hpIconHalf.Visible = true;
		}else{
			hpIconFull.Visible = true;
		}
	}

	public void UpdateSP(int newSP, int newMaxSP){
		SP.Text = "" + newSP;
		maxSPLabel.Text = "" + newMaxSP;
		//Updates the little icon next to the SP number
		spIconFull.Visible = false;
		spIconHalf.Visible = false;
		spIconEmpty.Visible = false;
		if(newSP <= 0){
			spIconEmpty.Visible = true;
		}else if(newSP <= newMaxSP/2){
			spIconHalf.Visible = true;
		}else{
			spIconFull.Visible = true;
		}
	}

	public void UpdateStatus(List<PMStatus> statuses){
		foreach(TextureRect statusIcon in statusBar.GetChildren()){
			statusIcon.Free();
		}
		if(statuses.Count > 3){
			foreach(PMStatus status in statuses){
				statusBar.AddChild(status.GetShortIcon());
			}
		}else{
			foreach(PMStatus status in statuses){
				statusBar.AddChild(status.GetLongIcon());
			}
		}
	}

	public void EnableHighlight(){
		highLight.Visible = true;
	}

	public void DisableHighlight(){
		highLight.Visible = false;
	}
}
