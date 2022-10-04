using Godot;
using System;
using System.Collections.Generic;

public class PMPlayerCharacter : PMCharacter{
    List<PackedScene> abilitiesKnown = new List<PackedScene>();
    PMPlayerAbility[] abilitiesPrepared = new PMPlayerAbility[4];
    PMPlayerAbility basicAttack;
    public override void _Ready()
    {
        base._Ready();
        basicAttack = (PMPlayerAbility) GetNode("Basic Attack");
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Idle");
    } 
    public PMPlayerAbility GetBasicAttack(){
        return basicAttack;
    }
}