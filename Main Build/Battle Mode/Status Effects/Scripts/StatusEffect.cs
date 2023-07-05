using Godot;
using System;

public partial class StatusEffect : CombatAction
{
	[Export]
	protected int startingDuration;
	protected int remainingDuration;
	//The status does what it's supposed to, decrements its duration, then returns whether or not it is finished

	[Export(PropertyHint.File)]
	protected string notificationNodeFilePath;
	protected PackedScene notification;

	public override void _Ready(){
		base._Ready();
		notification = GD.Load<PackedScene>(notificationNodeFilePath);
	}

	public override void AnimationTrigger(int phase){
		if(remainingDuration != -1){
			remainingDuration --;
			if(remainingDuration <= 0){
				source.LogExpiredStatus(this);
				this.QueueFree();
			}
		}
	}

	public BattleNotification ShowNotification(){
		if(notification == null) notification = GD.Load<PackedScene>(notificationNodeFilePath);
		BattleNotification note = notification.Instantiate<BattleNotification>();
		source.GetBodyRegion(0).AddChild(note);
		return note;
	}
}
