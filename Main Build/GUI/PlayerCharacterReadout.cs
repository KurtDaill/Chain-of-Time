using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerCharacterReadout : TextureRect
{
	[Export]
	protected Label HP;
	[Export]
	protected Label SP;
	[Export]
	protected Label maxHPLabel;
	[Export]
	protected Label maxSPLabel;
	[Export]
	protected TextureProgressBar hpBar;
	[Export]
	protected TextureProgressBar spBar;
	public PlayerCombatant character;
	[Export]
	protected TextureRect highlight;
    public override void _Ready()
	{
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

	public virtual void Select(){
		highlight.Visible = true;
		SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
	}

	public virtual void Deselect(){
		highlight.Visible = false;
		SizeFlagsVertical = Control.SizeFlags.ShrinkEnd;
	}
}
