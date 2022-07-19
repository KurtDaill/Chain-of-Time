using Godot;
using System;
using static ItemUtilities;

public abstract class Item
{
    public ItemType type;
    protected string name;
    public string GetName(){
        return name;
    }
}

public static class ItemUtilities{
    public enum ItemType{
        Quest,
        Puzzle,
        ProgressFlag,
        Battle
    }
}

public class DebugItem : Item 
{
    public DebugItem(){
        type = ItemType.Battle;
        name = "Debug Item";
    }
}
