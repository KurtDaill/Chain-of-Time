using Godot;
using System;

public partial class StatusEffect : Node
{
	protected int defaultStartingDuration, remainingDuration;

	protected Combatant target;
	//The status does what it's supposed to, decrements its duration, then returns whether or not it is finished
	public bool Execute(){
		remainingDuration --;
		if(remainingDuration <= 0){
			this.QueueFree();
			return true;
		}
		return false;
	}
}
