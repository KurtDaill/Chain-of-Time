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


    //[Export]
    //List<NodePath> debugAbilities = new List<NodePath>(); //TODO Temp Code, Remove when Battle Starts are Implemented
    PMPlayerAbility[] abilitiesPreparedInstanced = new PMPlayerAbility[4];
    
    [Export(PropertyHint.Flags)]
    string sceneFilePath;

    private int currentSP;

    [Export]
    private int maxSP;
    PMPlayerAbility basicAttackInstanced;

    PlayerCharacterReadout myReadout;
    [Export]
    NodePath readout; //Temp Code. TODO: Replace Me.
    public override void _Ready()
    {
        base._Ready();
        var basicInstanced = GD.Load<PackedScene>(basicAttack).Instance<PMPlayerAbility>();
        this.AddChild(basicInstanced);
        basicAttackInstanced = basicInstanced;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
        myReadout = GetNode<PlayerCharacterReadout>(readout);
        currentSP = maxSP;
        /*for(int i = 0 ; i < debugAbilities.Count; i++){ //TODO Temp Code, Remove when Battle Starts are Implemented
            if(debugAbilities[i] != null){
                abilitiesPreparedInstanced[i] = GetNode<PMPlayerAbility>(debugAbilities[i]);
            }
        }*/
        for(int i = 0; i < abilitiesPrepared.Length; i++){
            var instance = GD.Load<PackedScene>(allAbilities[abilitiesPrepared[i]]).Instance<PMPlayerAbility>();
            this.AddChild(instance);
            abilitiesPreparedInstanced[i] = instance; 
        }
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

    public void SetupReadout(){
        myReadout.UpdateHP(currentHP, maxHP);
        myReadout.UpdateSP(currentSP, maxSP);
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
}