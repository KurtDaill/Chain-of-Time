using Godot;
using System;
using System.Collections.Generic;
using static BattleUtilities;

public partial class Combatant : Node3D
{
	protected int HP;
	protected AnimationPlayer animPlay;

	protected BattlePosition currentPosition;

	protected List<StatusEffect> activeStatuses;

	protected string name = "defaultCombatantName";

	protected CombatAction readyAction;

	[Export]
	protected Sprite3D pointer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlay = (AnimationPlayer)this.GetNode("AnimationPlayer");
		activeStatuses = new List<StatusEffect>();
		//TODO Make a Better Version of this
		foreach(Node child in GetChildren()){
			if(child is Ability){
				Ability ab = (Ability)child;
				ab.Setup(this);
			}
			if(child is StatusEffect){
				StatusEffect stat = (StatusEffect)child;
				stat.Setup(this);
				activeStatuses.Add(stat);
			}
		}
		pointer.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public virtual void TakeDamage(int damage)
	{
		animPlay.Play("HitReact");
		this.HP -= damage;
		//Figure how how we're displaying damage numbers
		//Figure out how we're dealing with death logic
	}

	public bool HasAnimation(string name){
		var test = animPlay.GetAnimationList();
		return animPlay.HasAnimation(name);
	}

	public string GetName(){
		return name;
	}

	public AnimationPlayer GetAnimationPlayer(){
		return animPlay;
	}

	public BattlePosition GetPosition(){
		return currentPosition;
	}

	public void SetPosition(BattlePosition pos){
		currentPosition = pos;
	}

	public StatusEffect[] GetStatusEffects(){
		return activeStatuses.ToArray();
	}

	public void LogExpiredStatus(StatusEffect expiringStatus){
		this.activeStatuses.Remove(expiringStatus);
	}

	public void ReadyAction(CombatAction act){
		if(this.GetChildren().Contains(act)){
			readyAction = act;
		}else{
			throw new ActionNotFoundException("Action Failed to Ready. Action (" + act.GetName() + ") not found as child of Combatant (" + name + "). Actions must be childed to combatants to be readied or used.");
		}
	}

	//Returns all status effects that trigger on the start of turn (the upkeep)
	public StatusEffect[] GetUpkeepStatusEffects(){
		var result = new List<StatusEffect>(activeStatuses);
		result.RemoveAll(x => !(x is OnUpkeepStatus));
		return result.ToArray();
	}

	//Returns all status effect that trigger when the character acts
	public StatusEffect[] GetActStatusEffects(){
		var result = new List<StatusEffect>(activeStatuses);
		result.RemoveAll(x => !(x is OnActStatus));
		return result.ToArray();
	}

	public void ActivateReadyAction(int phase){
		readyAction.Activate(phase);
	}
	//Returns whether or not this character is able to input a command in order to act this turn.
	public bool IsAbleToAct(){
		//TODO: Flesh out this function with the game logic of when characters can/can't set a command for their actions this turn;
		return true;
	}

	//Used to activate/deactivate all of the effets that indicate the player is targeting this character in the battle menu
	//TODO Implement this
	public void SetTargetGUIElements(bool state){
		//Turn the Pointer On/Off
		pointer.Visible = state;
	}

	public class ActionNotFoundException : Exception
	{
		public ActionNotFoundException(){}
		public ActionNotFoundException(string message): base(message) {}
		public ActionNotFoundException(string message,Exception inner) : base(message,inner){}
	}
}
