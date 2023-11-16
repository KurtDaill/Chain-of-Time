using System;
using Godot;

public partial class WorkshopShopInterface : ShopInterface{
    protected override GameplayMode ProcessButtonPress(string activateString){
        switch(activateString){
            case "FindItem":
            //TODO actually pick a random item instead of just spawning in the Bru
                Item[] items = new Item[10];
                for(int i = 0; i < 10; i++){
                    if(i < 4) items[i] = GD.Load<PackedScene>("res://Battle Mode/Items/OrcishFireBrew.tscn").Instantiate() as Item;
                    else if(i < 8) items[i] = GD.Load<PackedScene>("res://Battle Mode/Items/IronSoulTablet.tscn").Instantiate() as Item;
                    else items[i] = GD.Load<PackedScene>("res://Battle Mode/Items/WhiteStoneSigil.tscn").Instantiate() as Item;
                }
                Random rando = new();
                int dieRoll = rando.Next(0,9);    
                
                if(gm.GetCurrentTU() > 0){
                    gm.SpendTU(1);
                    gm.GainItem(items[dieRoll]);
                    descritpionTextBox.Text = "Got " + items[dieRoll].GetDisplayName();
                }else{
                    //TODO Add A "There is no Time" Message
                }
                return null;
            case "GetEquipment":

                return null;
            case "RepairHouse":
                if(gm.GetCurrentTU() >= 3){
                    if(GetNode<CityState>("/root/CityState").RepairBuildingOutsideOfCityScene()){
                        gm.SpendTU(3);
                        descritpionTextBox.Text = "Took a while, but one more family oughta sleep soundly now.";
                    }
                }
                return null;
            default: throw new ArgumentException("Button Activation String Error. Button Activated String: " + activateString + " not recognized by Shop Interface: " + Name + ".");
        }
        return null;
    }
}