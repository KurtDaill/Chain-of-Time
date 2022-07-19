using Godot;
using System;
using System.Collections.Generic;
using static ItemUtilities;

public class PartyData //Implements the Singelton Pattern
{
    private static PartyData instance;
    private List<(Item, int)> inventory = new List<(Item, int)>();
    private PlayerCombatant[] activeCharacters = new PlayerCombatant[3];

    private int alignment  = 0;
    public static PartyData Instance(){
        if(instance == null) instance = new PartyData();
        return instance;
    }

    public PartyData(){
        LoadData();
    }

    private void LoadData(){
        //THIS IS TEMPORARY
        activeCharacters[0] = new CatoCombatant();
        for(int i = 0; i < 10; i++){
            inventory.Add((new DebugItem(), i));
        }
        //TODO: Implement LoadData 
        /*
            Load data from save files.
        */
    }

    public List<(Item, int)> GetItems(){
        return inventory;
    }

    public List<(Item, int)> GetBattleItems(){
        var temp = new List<(Item, int)>(inventory);        
        foreach((Item, int) stack in temp){
            if(stack.Item1.type != ItemType.Battle){
                temp.Remove(stack);
            }
        }
        return temp;
    }
}
