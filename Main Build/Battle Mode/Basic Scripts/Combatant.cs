using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static BattleUtilities;

public partial class Combatant : Node3D
{
	[Export]
	protected int hp;
	[Export]
	protected int maxHP;
	[Export]
	CombatText displayText;
	protected AnimationPlayer animPlay;

	protected BattlePosition currentPosition;

	protected List<StatusEffect> activeStatuses;
	protected List<CombatFX> combatVisEffects;

	[Export]
	protected PositionSwap swapAbility;
	protected string name = "defaultCombatantName";

	protected CombatAction readyAction;

	bool defeated = false;

	protected Node3D[] bodyRef;

	[Export]
	protected Sprite3D pointer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animPlay = (AnimationPlayer)this.GetNode("AnimationPlayer");
		animPlay.Play("Idle");
		animPlay.AnimationFinished += OnAnimationComplete;

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

		bodyRef = new Node3D[4]{
			GetNode<Node3D>("BodyRef/Head"),
			GetNode<Node3D>("BodyRef/Core"),
			GetNode<Node3D>("BodyRef/Weapon"),
			GetNode<Node3D>("BodyRef/Feet")
		};
		
		combatVisEffects = new List<CombatFX>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		/*
		if(hp < 0 && defeated == false){
			defeated = true;
			await ToSignal(animPlay, AnimationPlayer.SignalName.AnimationFinished);
			animPlay.Play("GoDown");
		}
		if(defeated && hp > 0){
			defeated = false;
			//Go Back to Idle
		}
		*/
	}

	public virtual void TakeDamage(int damage)
	{
		animPlay.Play("HitReact");
		//if we have any armor statuses, calculate damage reduction due to armor
		if(this.GetStatusEffects().Where(x => x is StatusArmor).Count() != 0){
			List<StatusArmor> armorStatuses = new List<StatusArmor>();
			foreach(StatusEffect effect in this.GetStatusEffects().Where(x => x is StatusArmor)){armorStatuses.Add((StatusArmor) effect);}
			int armorValue = 0;
			foreach(StatusArmor armor in armorStatuses){if(armor.GetArmorValue() > armorValue) armorValue = armor.GetArmorValue();}
			damage = Math.Max(1, (damage - armorValue));
		}
		this.hp -= damage;
		displayText.ShowDamage(damage);
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

	public int GetHP(){
		return hp;
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

	public void GainStatus(StatusEffect status){
		this.AddChild(status);
		status.Setup(this);
		this.activeStatuses.Add(status);
	}

	public StatusEffect[] GetStatusEffects(){
		return activeStatuses.ToArray();
	}

	public void LogExpiredStatus(StatusEffect expiringStatus){
		this.activeStatuses.Remove(expiringStatus);
	}

	public void ReadyAction(CombatAction act, Battle battle){
		if(this.GetChildren().Contains(act)){
			readyAction = act;
			act.SetBattle(battle);
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
		readyAction.AnimationTrigger(phase);
	}
	//Returns whether or not this character is able to input a command in order to act this turn.
	public bool IsAbleToAct(){
		//TODO: Flesh out this function with the game logic of when characters can/can't set a command for their actions this turn;
		return true;
	}

	//Used to activate/deactivate all of the effets that indicate the player is targeting this character in the battle menu
	//TODO Implement this
	public virtual void SetTargetGUIElements(bool state){
		//Turn the Pointer On/Off
		pointer.Visible = state;
	}

	public virtual void DefeatMe(){
		animPlay.Play("GoDown");
		defeated = true;
	}

	public virtual void ReviveMe(){
		defeated = false;
	}

	public bool IsAlreadyDefeated(){
		return defeated;
	}

	public Node3D GetBodyRegion(int index){
		return bodyRef[index];
	}

	public void AddCombatFX(CombatFX newFX){
		this.AddChild(newFX);
		combatVisEffects.Add(newFX);
		newFX.SetSource(this);
	}

	public void LogExpiredCombatFX(CombatFX removedFX){
		combatVisEffects.Remove(removedFX);
	}

	//Called when an animation is complete, should always lead us back to the current idle.
	protected virtual void OnAnimationComplete(StringName animName){
		//TODO: Include Logic for Common Alt Idles: Bloodied, Dead, etc.
		this.animPlay.Play("Idle");
	}

	public class ActionNotFoundException : Exception
	{
		public ActionNotFoundException(){}
		public ActionNotFoundException(string message): base(message) {}
		public ActionNotFoundException(string message,Exception inner) : base(message,inner){}
	}
}
