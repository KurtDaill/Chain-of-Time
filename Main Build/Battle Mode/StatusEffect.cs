using Godot;
using System;

public partial class StatusEffect : CombatAction
{
	protected int defaultStartingDuration, remainingDuration;
	//The status does what it's supposed to, decrements its duration, then returns whether or not it is finished
	public override void Activate(int phase){
		remainingDuration --;
		if(remainingDuration <= 0){
			source.LogExpiredStatus(this);
			this.QueueFree();
			//return true;
		}
		//return false;
	}
}
