using Godot;
using System;
using static ItemUtilities;
public abstract partial class Item : Node
{
    [Export]
    protected string displayName;
    protected ItemType type;
    protected EquipSlot equipSlot;
    [Export]
    protected Texture2D icon;
    [Export]
    protected string rulesText;

    public string GetDisplayName(){
        return displayName;
    }
    public ItemType GetItemType(){
        return type;
    }
    public EquipSlot GetEquipSlot(){
        return equipSlot;
    }
    public Texture2D GetIcon(){
        return icon;
    }
}

public static class ItemUtilities{
    public enum ItemType{
        Consumable,
        Equipable
    }

    public enum EquipSlot{
        None,
        Weapon,
        Armor,
        Trinket
    }
}