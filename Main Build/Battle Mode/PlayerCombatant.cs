using Godot;
using System;
using System.Collections.Generic;
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

	// Called when the node enters the scene tree for the first time.
	public async override void _Ready()
	{
		base._Ready();
		await ToSignal(readout.GetParent(), ReadoutContainer.SignalName.ReadyToPopulateReadouts);
		readout.UpdateHP(hp, maxHP);
		readout.UpdateSP(sp, maxSP);
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

	public void GainSP(int gain){
		sp += gain;
		if(sp > maxSP) sp = maxSP;
	}

	public PlayerAbility GetBasicAttack(){
		return basicAttack;
	}
}
