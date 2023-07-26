using Godot;
using System;
using System.Threading.Tasks;

public partial class CombatText : Node3D
{
	private AnimationPlayer animPlay;
	[Export]
	private Label3D healingLabel;
	[Export]
	private Label3D damageLabel;
	[Export]
	private Label3D spLabel;

	public override void _Ready(){
		animPlay = GetNode<AnimationPlayer>("AnimationPlayer");
	}
	public async void ShowHealing(int value){
		if(animPlay.IsPlaying()){
			await ResetTextAnimation();
		}
		healingLabel.Text = "" + value;
		animPlay.Play("Heal");
	}

	public async void ShowDamage(int value){
		if(animPlay.IsPlaying()){
			await ResetTextAnimation();
		}
		damageLabel.Text = "" + value;
		animPlay.Play("Damage");
	}

	public async void ShowSP(int value){
		if(animPlay.IsPlaying()){
			await ResetTextAnimation();
		}
		spLabel.Text = "" + value;
		animPlay.Play("GainSP");
	}

	public async Task ResetTextAnimation(){
		animPlay.Play("RESET");
		await ToSignal(animPlay, AnimationPlayer.SignalName.AnimationFinished);
		return;
	}
}
