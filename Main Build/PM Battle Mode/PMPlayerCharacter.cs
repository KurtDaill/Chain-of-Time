using Godot;
using System;
using System.Collections.Generic;

public class PMPlayerCharacter : PMCharacter{
    List<PackedScene> abilitiesKnown = new List<PackedScene>();
    
    [Export]
    List<NodePath> debugAbilities = new List<NodePath>(); //TODO Temp Code, Remove when Battle Starts are Implemented
    PMPlayerAbility[] abilitiesPrepared = new PMPlayerAbility[4];

    private int currentSP;

    [Export]
    private int maxSP;
    PMPlayerAbility basicAttack;

    PlayerCharacterReadout myReadout;
    [Export]
    NodePath readout; //Temp Code. TODO: Replace Me.
    public override void _Ready()
    {
        base._Ready();
        basicAttack = (PMPlayerAbility) GetNode("Basic Attack");
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
        myReadout = GetNode<PlayerCharacterReadout>(readout);
        currentSP = maxSP;
        for(int i = 0 ; i < debugAbilities.Count; i++){ //TODO Temp Code, Remove when Battle Starts are Implemented
            if(debugAbilities[i] != null){
                abilitiesPrepared[i] = GetNode<PMPlayerAbility>(debugAbilities[i]);
            }
        }
    } 
    public PMPlayerAbility GetBasicAttack(){
        return basicAttack;
    }

    public override void TakeDamage(int damage, PMBattleUtilities.AbilityAlignment alignment)
    {
        base.TakeDamage(damage, alignment);
        myReadout.UpdateHP(currentHP, MaxHP);
    }

    public override void TakeHealing(int heal, PMBattleUtilities.AbilityAlignment alignment)
    {
        base.TakeHealing(heal, alignment);
        myReadout.UpdateHP(currentHP, MaxHP);
    }

    public void PlayDefenseAnimation(){
        animPlay.Play("Defend");
    }

    public void SetupReadout(){
        myReadout.UpdateHP(currentHP, MaxHP);
        myReadout.UpdateSP(currentSP, maxSP);
    }

    public PMPlayerAbility[] GetAbilities(){
        return abilitiesPrepared;
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
}