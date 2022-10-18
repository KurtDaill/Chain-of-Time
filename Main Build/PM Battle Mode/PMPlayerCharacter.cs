using Godot;
using System;
using System.Collections.Generic;

public class PMPlayerCharacter : PMCharacter{
    List<PackedScene> abilitiesKnown = new List<PackedScene>();
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
    } 
    public PMPlayerAbility GetBasicAttack(){
        return basicAttack;
    }

    public override void TakeDamage(int damage, PMBattleUtilities.AbilityAlignment alignment)
    {
        base.TakeDamage(damage, alignment);
        myReadout.UpdateHP(currentHP, MaxHP);
    }

    public void SetupReadout(){
        myReadout.UpdateHP(currentHP, MaxHP);
        myReadout.UpdateSP(currentSP, maxSP);
    }
}