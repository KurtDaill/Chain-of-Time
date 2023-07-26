using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public partial class PlayerCombatant : Combatant
{
	[Export]
	protected int sp;
	[Export]
	protected int maxSP;

	//Includes a dicitonary of potential damage done by a basic attack (expressed in an int), and the probability of that ammount of damage (expressed in a double)
	//The doubles should add up to one.
	protected Dictionary<double, int> basicAttackDamageRange;
	[Export]
	protected PlayerAbility basicAttack;
	[Export]
	protected PlayerCharacterReadout readout;
	[Export]
	protected Texture2D displayPortrait;
	protected PlayerSkill[] readySkills;
	[Export]
	protected PlayerSwap swapAbility;

	// Called when the node enters the scene tree for the first time.
	public async override void _Ready()
	{
		readySkills = new PlayerSkill[4];
		base._Ready();
		foreach(Node child in GetChildren()){
			if(child is PlayerSkill) readySkills[readySkills.Count(x => x != null)] = (PlayerSkill) child;
		}
		await ToSignal(readout.GetParent(), ReadoutContainer.SignalName.ReadyToPopulateReadouts);
		readout.UpdateHP(hp, maxHP);
		readout.UpdateSP(sp, maxSP);
		readout.character = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	//Implement all of the effects that indicate that this player character is currenly selected for the player to input their commands for
	public void SelectMe(){

	}

	//Undoes whatever SelectMe Does
	public void UnselectMe(){

	}

	public void GainSP(int gain, bool showInUI = true){
		sp += gain;
		if(sp > maxSP) sp = maxSP;
		readout.UpdateSP(sp, maxSP);
		if(showInUI) displayText.ShowSP(gain);
	}

	public bool ChargeSP(int cost){
		if(sp < cost) return false;
		sp -= cost;
		readout.UpdateSP(sp, maxSP);
		return true;
	}

	public PlayerAbility GetBasicAttack(){
		return basicAttack;
	}
	
	public override void TakeDamage(int damage){
		base.TakeDamage(damage);
		readout.UpdateHP(hp, maxHP);
	}

	public PlayerSkill[] GetSkills(){
		return readySkills.Where(x => x != null).ToArray();
	}

	public Texture2D GetPortrait(){
		return displayPortrait;
	}

	public PlayerData GetPlayerData(){
		return new PlayerData(name, hp, maxHP, sp, maxSP);
	}

	public void LoadPlayerData(PlayerData load){
		this.name = load.GetName();
		this.hp = load.GetHP();
		this.maxHP = load.GetMaxHP();
		this.sp = load.GetSP();
		this.maxSP = load.GetMaxSP();
		readout.UpdateHP(hp, maxHP);
		readout.UpdateSP(sp, maxSP);
	}

	public PlayerAbility SetupAndGetSwap(Roster ros, BattlePosition target){
		swapAbility.SetTargets(new Combatant[1]{ros.GetCombatant(target)});
		swapAbility.SetupSwapDetails(ros, target);
		return swapAbility;
	}
}
