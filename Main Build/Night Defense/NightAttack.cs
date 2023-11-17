using System;
using System.Collections.Generic;
using Godot;
public partial class NightAttack : Resource{
    [Export]
    Godot.Collections.Array<PackedScene> definiteEnemyEncounters;
    [Export]
    Godot.Collections.Dictionary<float, PackedScene> randomChanceEncounters;

    //Called from Night Defense on load, meant to check whether something illegal has been placed in one of the data structures
    public void CheckIntegrity(){
        foreach(PackedScene pack in definiteEnemyEncounters){
            Node instance = pack.Instantiate();
            if(!(instance is EnemyGroup)) throw new ArgumentException("The Values in Night Attacks's Encounters must be Enemy Groups as Packed Scenes");
        }
        foreach(KeyValuePair<float, PackedScene> pair in randomChanceEncounters){
            Node instance = pair.Value.Instantiate();
            if(pair.Value is PackedScene && instance is EnemyGroup){
                instance.Free();
                if(pair.Key > 1 || pair.Key < 0){
                    throw new ArgumentException("the double value for random encounters must be between 0 & 1");
                }
            }else{
                throw new ArgumentException("The Values in NightAttack's Encounter Dictionary Must be Enemy Groups as Packed Scenes");
            }
        }
    }

    public Godot.Collections.Array<PackedScene> GetEnemyGroups(){
        Godot.Collections.Array<PackedScene> finalEnemyGroups = definiteEnemyEncounters.Duplicate();
        Random rand = new();
        foreach(KeyValuePair<float, PackedScene> pair in randomChanceEncounters){
            if(rand.NextDouble() >= pair.Key) finalEnemyGroups.Add(pair.Value);
        }
        return finalEnemyGroups;
    }
}