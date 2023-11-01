using System;
using Godot;

public partial class WorkshopShopInterface : ShopInterface{
    protected override GameplayMode ProcessButtonPress(string activateString){
        switch(activateString){
            case "FindItem":
            //TODO actually pick a random item instead of just spawning in the Bru
                Item item = GD.Load<PackedScene>("res://Battle Mode/Items/OrcishFireBrew.tscn").Instantiate() as Item;
                
                if(gm.GetCurrentTU() > 0){
                    gm.SpendTU(1);
                    gm.GainItem(item);
                    descritpionTextBox.Text = "Got " + item.GetDisplayName();
                }else{
                    //TODO Add A "There is no Time" Message
                }
                return null;
            case "GetEquipment":

                return null;
            default: throw new ArgumentException("Button Activation String Error. Button Activated String: " + activateString + " not recognized by Shop Interface: " + Name + ".");
        }
        return null;
    }
}