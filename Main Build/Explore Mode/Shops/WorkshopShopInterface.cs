using System;
using System.Collections.Generic;
using Godot;
using static GameplayUtilities;
using System.Linq;

public partial class WorkshopShopInterface : ShopInterface{
    private bool inItemSelect = false;
    [Export]
    Node3D itemSelectSubMenu;
    [Export]
    Godot.Collections.Array<Sprite3D> itemSelectIcons;
    [Export]
    RichTextLabel itemDescriptionBox;
    ConsumableItem[] itemsRolledForSelection;
    [Export]
    Godot.Collections.Array<PackedScene> junkItems;
    [Export]
    Godot.Collections.Array<PackedScene> commonItems;
    [Export]
    Godot.Collections.Array<PackedScene> rareItems;
    [Export(PropertyHint.Range, "0,1,0.1")]
    float chaceToGetaRare;
    [Export]
    Marker3D itemRackFrontMarker;
    [Export]
    Marker3D itemRackBackMarker;
    [Export]
    SpotLight3D itemSelectSpotLight;
    [Export]
    Camera3D itemSelectCam;
    int selectedItem = 0;

    public override void _Ready()
    {
        base._Ready();
        itemSelectSubMenu.Visible = false;
    }
    
    //private delegate void PlayerSelectedItemEventHandler(ConsumableItem item);
    protected override GameplayMode ProcessButtonPress(string activateString){
        switch(activateString){
            case "FindItem":
            if(gm.GetCurrentTU() > 0){
                gm.SpendTU(1);
            }else{
                return null;
            }
            inItemSelect = true;
            List<ConsumableItem> rolledItems = new();
            Random rando = new();
            rolledItems.Add(commonItems.OrderBy(a => rando.Next()).Cast<PackedScene>().ToArray()[0].Instantiate<ConsumableItem>());
            rolledItems.Add(junkItems.OrderBy(a => rando.Next()).Cast<PackedScene>().ToArray()[0].Instantiate<ConsumableItem>());
            rolledItems.OrderBy(a => rando.Next()).ToList();
            if(rando.NextDouble() <= (double)chaceToGetaRare){
                rolledItems.Add(rareItems.OrderBy(a => rando.Next()).Cast<PackedScene>().ToArray()[0].Instantiate<ConsumableItem>());
            }else{
                rolledItems.Add(commonItems.OrderBy(a => rando.Next()).Cast<PackedScene>().ToArray()[0].Instantiate<ConsumableItem>());
                rolledItems.OrderBy(a => rando.Next()).ToList();
            }
            itemsRolledForSelection = rolledItems.ToArray();
            for(int i = 0; i < itemsRolledForSelection.Length; i++){
                itemSelectIcons[i].Texture = itemsRolledForSelection[i].GetIcon();
            }
            itemSelectSubMenu.Visible = true;
            selectedItem = 0;
            itemSelectCam.Current = true;
            /*
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
                */
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
    }

    
    public override void HandleInput(PlayerInput input)
    {
        if(inItemSelect){
            switch(input){
                case PlayerInput.Right :
                    if(selectedItem < 2) selectedItem++;
                    SetItemSelect(selectedItem);
                    break;
                case PlayerInput.Left :
                    if(selectedItem > 0) selectedItem--;
                    SetItemSelect(selectedItem);
                    break;
                case PlayerInput.Select :
                    //Actually buy the Item
                    //Add Selected Item to Player Inventory
                    gm.GainItem(itemsRolledForSelection[selectedItem]);
                    descritpionTextBox.Text = "Got " + itemsRolledForSelection[selectedItem].GetDisplayName();
                    shopCam.Current = true;
                    inItemSelect = false;
                    break;
            }
        }else{
            itemSelectSubMenu.Visible = false;
            base.HandleInput(input);
        }
    }

    public void SetItemSelect(int i){
        itemDescriptionBox.Text = itemsRolledForSelection[i].GetDisplayName() + "\n" + itemsRolledForSelection[i].GetRulesText();
        for(int j = 0; j < itemSelectIcons.Count; j++){
            if(j == i){
                itemSelectIcons[j].GlobalPosition = new Vector3(itemSelectIcons[j].GlobalPosition.X, itemSelectIcons[j].GlobalPosition.Y, itemRackFrontMarker.GlobalPosition.Z); 
                itemSelectSpotLight.GlobalPosition = new Vector3(itemSelectIcons[i].GlobalPosition.X, itemSelectSpotLight.GlobalPosition.Y, itemSelectSpotLight.GlobalPosition.Z);
            }
            else itemSelectIcons[j].GlobalPosition = new Vector3(itemSelectIcons[j].GlobalPosition.X, itemSelectIcons[j].GlobalPosition.Y, itemRackBackMarker.GlobalPosition.Z); 
        }
    }
}