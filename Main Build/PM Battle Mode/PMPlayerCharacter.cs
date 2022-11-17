using Godot;
using System;
using System.Collections.Generic;
using static GameMaster;
public class PMPlayerCharacter : PMCharacter{
    [Export]
    int[] abilitiesKnown = new int[0];
    [Export]
    int[] abilitiesPrepared = new int[0];
    
    [Export(PropertyHint.File)]
    string[] allAbilities;
    [Export(PropertyHint.File)]
    string basicAttack;
    bool init = true;



    //[Export]
    //List<NodePath> debugAbilities = new List<NodePath>(); //TODO Temp Code, Remove when Battle Starts are Implemented
    PMPlayerAbility[] abilitiesPreparedInstanced = new PMPlayerAbility[4];
    
    [Export(PropertyHint.Flags)]
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
        /*for(int i = 0 ; i < debugAbilities.Count; i++){ //TODO Temp Code, Remove when Battle Starts are Implemented
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
        //parentBattle.GetNode("Camera/BattleGUI/Readouts").Connect("ReadyToPopulateReadouts", this, nameof(SetupGUI));
        //SetupGUI((ReadoutContainer)GetNode("Camera/BattleGUI/Readouts"));
    }

    public void SetupGUI(ReadoutContainer readouts){ //TODO: Check whether we need this later
        myReadout = GD.Load<PackedScene>(readout).Instance<PlayerCharacterReadout>();
        readouts.AddChild(myReadout);
        myReadout.character = this;
        myReadout.UpdateHP(currentHP, maxHP);
        myReadout.UpdateSP(currentSP, maxSP);
        readouts.Reorder();
        //SetupReadout();
    }
    public PMPlayerAbility GetBasicAttack(){
        return basicAttackInstanced;
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
        for(int i = 0; i < abilitiesPrepared.Length; i++){
            var instance = GD.Load<PackedScene>(allAbilities[abilitiesPrepared[i]]).Instance<PMPlayerAbility>();
            this.AddChild(instance);
            abilitiesPreparedInstanced[i] = instance; 
        }
        var basicInstanced = GD.Load<PackedScene>(basicAttack).Instance<PMPlayerAbility>();
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
            abilitiesKnown,
            abilitiesPrepared
        );
    }

    public void ImportData(PlayerCharacterData loadMe){
        this.currentHP = loadMe.hp;
        this.maxHP = loadMe.maxHP;
        this.currentSP = loadMe.sp;
        this.maxSP = loadMe.maxSP;
        this.myPosition = (PMBattleUtilities.BattlePos)loadMe.position;
        this.abilitiesKnown = loadMe.abilitiesKnown;
        this.abilitiesPrepared = loadMe.abilitiesPrepared;
    }
    
    public void SetupReadout(){
        myReadout.UpdateHP(currentHP, maxHP);
        myReadout.UpdateSP(currentSP, maxSP);
    }
}