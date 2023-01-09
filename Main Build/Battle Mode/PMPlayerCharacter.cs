using Godot;
using System;
using System.Collections.Generic;
using static GameMaster;
using static PMCharacterUtilities;
using System.Linq;
public partial class PMPlayerCharacter : PMCharacter{
	[Export]
	Godot.Collections.Array<int> abilitiesKnown = new Godot.Collections.Array<int>();
	[Export]
	Godot.Collections.Array<int> abilitiesPrepared = new Godot.Collections.Array<int>();
	
	[Export]
	Godot.Collections.Array<string> allAbilities;
	[Export(PropertyHint.File)]
	string basicAttack;
	bool init = true;



	//[Export]
	//List<NodePath> debugAbilities = new List<NodePath>(); //TODO Temp Code, RemoveAt when Battle Starts are Implemented
	PMPlayerAbility[] abilitiesPreparedInstanced = new PMPlayerAbility[4];
	
	[Export(PropertyHint.File)]
	string sceneFilePath;

	private int currentSP = -1;

	[Export]
	private int maxSP;
	PMPlayerAbility basicAttackInstanced;

	PlayerCharacterReadout myReadout;
	[Export(PropertyHint.File)]
	string readout;
	public override void _Ready()
	{
		base._Ready();
		GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
		if(currentSP == -1) currentSP = maxSP;
		/*for(int i = 0 ; i < debugAbilities.Count; i++){ //TODO Temp Code, RemoveAt when Battle Starts are Implemented
			if(debugAbilities[i] != null){
				abilitiesPreparedInstanced[i] = GetNode<PMPlayerAbility>(debugAbilities[i]);
			}
		}*/
		LoadAbilties();
		switch(GetParent().Name){ //TODO Better way of assigning position during this function
			case "Hero 1" :
			myPosition = PMBattleUtilities.BattlePos.HeroOne;
				break;
			case "Hero 2" :
			myPosition = PMBattleUtilities.BattlePos.HeroTwo;
				break;
			case "Hero 3" :
			myPosition = PMBattleUtilities.BattlePos.HeroThree;
				break;
		}
		//parentBattle.GetNode("Camera3D/BattleGUI/Readouts").Connect("ReadyToPopulateReadouts",new Callable(this,nameof(SetupGUI)));
		//SetupGUI((ReadoutContainer)GetNode("Camera3D/BattleGUI/Readouts"));
	}

	public void SetupGUI(ReadoutContainer readouts){ //TODO: Check whether we need this later
		myReadout = GD.Load<PackedScene>(readout).Instantiate<PlayerCharacterReadout>();
		readouts.AddChild(myReadout);
		myReadout.character = this;
		myReadout.UpdateHP(currentHP, maxHP);
		myReadout.UpdateSP(currentSP, maxSP);
		readouts.Reorder();
		//This line is included to notify the readout of any statuses the character starts with
		myReadout.UpdateStatus(statusEffects);
		//SetupReadout();
	}
	public PMPlayerAbility GetBasicAttack(){
		return basicAttackInstanced;
	}
	
	public override void AddStatus(PMStatus newEffect){
		base.AddStatus(newEffect);
	} 

	public override void RemoveStatus(PMStatus removeEffect){
		base.RemoveStatus(removeEffect);
		myReadout.UpdateStatus(statusEffects);
	}
	public override void TakeDamage(int damage, PMBattleUtilities.AbilityAlignment alignment)
	{
		base.TakeDamage(damage, alignment);
		myReadout.UpdateHP(currentHP, maxHP);
	}

	public override void TakeHealing(int heal, PMBattleUtilities.AbilityAlignment alignment)
	{
		base.TakeHealing(heal, alignment);
		myReadout.UpdateHP(currentHP, maxHP);
	}

	public void PlayDefenseAnimation(){
		animPlay.Play("Defend");
	}

	public void PlayBlockAnimation(){
		animPlay.Play("Block");
	}
	public PMPlayerAbility[] GetAbilities(){
		return abilitiesPreparedInstanced;
	}

	public bool ChargeSP(int cost){
		if(cost > currentSP) return false;
		else{
			currentSP -= cost;
			myReadout.UpdateSP(currentSP, maxSP);
			return true;
		}
	}

	public void DrainSP(int drain){
		currentSP -= drain;
		myReadout.UpdateSP(currentSP, maxSP);
	}

	public void GainSP(int gain){
		currentSP += gain;
		myReadout.UpdateSP(currentSP, maxSP);
	}

	public void SelectMe(){
		animPlay.Play("Excited Idle");
		myReadout.EnableHighlight();
	}

	public void UnselectMe(){
		animPlay.Play("Idle");
		myReadout.DisableHighlight();
	}

	public void LoadAbilties(){
		//Clear any already loaded abilties
		foreach(Node child in this.GetChildren()){
			if(child is PMPlayerAbility){
				this.RemoveChild(child);
				child.QueueFree();
			}
		}
		//Load the abilites that should be present
		if(allAbilities.Count != 0){
			for(int i = 0; i < abilitiesPrepared.Count; i++){
			var instance = ResourceLoader.Load<PackedScene>(allAbilities[abilitiesPrepared[i]]).Instantiate<PMPlayerAbility>();
			this.AddChild(instance);
			abilitiesPreparedInstanced[i] = instance; 
			}
		}
		var basicInstanced = ResourceLoader.Load<PackedScene>(basicAttack).Instantiate<PMPlayerAbility>();
		this.AddChild(basicInstanced);
		basicAttackInstanced = basicInstanced;
	}

	public PlayerCharacterData ExportData(){
		return new PlayerCharacterData(
			sceneFilePath,
			currentHP,
			maxHP,
			currentSP,
			maxSP,
			(uint) myPosition,
			abilitiesKnown.ToArray<int>(),
			abilitiesPrepared.ToArray<int>()
		);
	}

	public void ImportData(PlayerCharacterData loadMe){
		this.currentHP = loadMe.hp;
		this.maxHP = loadMe.maxHP;
		this.currentSP = loadMe.sp;
		this.maxSP = loadMe.maxSP;
		this.myPosition = (PMBattleUtilities.BattlePos)loadMe.position;
		this.abilitiesKnown = new Godot.Collections.Array<int>(loadMe.abilitiesKnown);
		this.abilitiesPrepared = new Godot.Collections.Array<int>(loadMe.abilitiesPrepared);
	}
	
	public void SetupReadout(){
		myReadout.UpdateHP(currentHP, maxHP);
		myReadout.UpdateSP(currentSP, maxSP);
	}
}
