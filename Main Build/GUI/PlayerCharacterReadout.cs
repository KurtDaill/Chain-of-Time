using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerCharacterReadout : TextureRect
{
	Label HP, SP;
	Label maxHPLabel, maxSPLabel;
	TextureProgressBar hpBar, spBar;

	HBoxContainer statusBar;
	public PlayerCombatant character;
	TextureRect highlight;

	public override void _Ready()
	{
		HP = GetNode<Label>("HP");
		SP = GetNode<Label>("SP");
		maxHPLabel = GetNode<Label>("MaxHP");
		maxSPLabel = GetNode<Label>("MaxSP");
		hpBar = GetNode<TextureProgressBar>("HPBar");
		spBar = GetNode<TextureProgressBar>("SPBar");
		highlight = GetNode<TextureRect>("Highlight");
		highlight.Visible = false;
		Deselect();
	}
	
	public void UpdateHP(int newHP, int newMaxHP){
		HP.Text = "" + newHP;
		maxHPLabel.Text = "" + newMaxHP;
		hpBar.MaxValue = newMaxHP;
		hpBar.Value = newHP;
	}

	public void UpdateSP(int newSP, int newMaxSP){
		SP.Text = "" + newSP;
		maxSPLabel.Text = "" + newMaxSP;
		spBar.MaxValue = newMaxSP;
		spBar.Value = newSP;
	}

	public void UpdateStatus(List<StatusEffect> statuses){
		/*foreach(TextureRect statusIcon in statusBar.GetChildren()){
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
		}*/
	}

	public void Select(){
		highlight.Visible = true;
		SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
	}

	public void Deselect(){
		highlight.Visible = false;
		SizeFlagsVertical = Control.SizeFlags.ShrinkEnd;
	}
}
