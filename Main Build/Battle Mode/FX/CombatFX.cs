using Godot;
using System;

public partial  class CombatFX : Node3D
{
	[Export]
	private bool oneShot;
	[Export]
	private string coreAnimationName;

	private AnimationPlayer animPlay;

	protected Combatant source;

	public override async void _Ready(){
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
		animPlay.Play(coreAnimationName);
		if(oneShot){
			await ToSignal(animPlay, AnimationPlayer.SignalName.AnimationFinished);
			End();
		}
	}

	public bool IsOneShot(){return oneShot;}
	public Combatant GetSource(){return source;}
	public void SetSource(Combatant com){source = com;}
	protected void End(){
		source.LogExpiredCombatFX(this);
		this.QueueFree();
	}
}
