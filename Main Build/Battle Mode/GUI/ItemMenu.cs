using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static GameplayUtilities;
public partial class ItemMenu : BattleMenu
{
    private ConsumableItem[] itemsAvailable;
    private TextureRect[] itemTabs;
    private int currentItem;

    private TextureRect itemIcon;
    private RichTextLabel titleLineLabel, descriptionLabel;
    public override void _Ready(){
        itemTabs = new TextureRect[4]{
            GetNode<TextureRect>("Item TabBar/Item Tab 1"),
            GetNode<TextureRect>("Item TabBar/Item Tab 2"),
            GetNode<TextureRect>("Item TabBar/Item Tab 3"),
            GetNode<TextureRect>("Item TabBar/Item Tab 4")
        };
        itemIcon = this.GetNode<TextureRect>("Item Description Box/Item Icon");
        descriptionLabel = this.GetNode<RichTextLabel>("Item Description Box/Description Label");
        titleLineLabel = this.GetNode<RichTextLabel>("Item Description Box/Title Line Label");
        this.Visible = false;
    }
    public override void OnOpen(PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        base.OnOpen(character, caller, parentGUI);
        //Load in all the items the party has
        //itemsAvailable = new ConsumableItem[caller.GetRoster().GetChildren().Where(x => x is ConsumableItem).ToArray().Length];
        var temp = new List<ConsumableItem>();
        foreach(Node node in caller.GetRoster().GetNode("Items").GetChildren().Where(x => x is ConsumableItem).ToArray()){
            temp.Add((ConsumableItem) node);
        }
        itemsAvailable = temp.ToArray();
        //Set our Display to reflect that
        for(int i = 0; i < itemTabs.Length; i++){
            if(itemsAvailable.Length > i){
                itemTabs[i].Visible = true;   
                itemTabs[i].GetNode<Label>("Name").Text = itemsAvailable[i].GetDisplayName();
            }else{
                itemTabs[i].Visible = false;
            }
        }
    }

    public void OnOpenInsidePauseMenu(){
        var temp = this.GetNode<GameMaster>("/root/GameMaster").GetInventory().Where(x => x is ConsumableItem).ToList();
        itemsAvailable = new ConsumableItem[temp.Count];
        for(int i = 0; i < temp.Count; i ++){
            itemsAvailable[i] = (ConsumableItem)temp[i];
        }
        //Set our Display to reflect that
        for(int i = 0; i < itemTabs.Length; i++){
            if(itemsAvailable.Length > i){
                itemTabs[i].Visible = true;   
                itemTabs[i].GetNode<Label>("Name").Text = itemsAvailable[i].GetDisplayName();
            }else{
                itemTabs[i].Visible = false;
            }
        }
    }

    public void HandleInputPauseMenu(PlayerInput input){
        switch(input){
            case PlayerInput.Up:
                if(currentItem > 0) currentItem--;
                break;
            case PlayerInput.Down:
                if(currentItem < itemsAvailable.Length - 1) currentItem++;
                break;
        }
        if(input != PlayerInput.None){
            SetHighlight();
            SetDescriptionBox(itemsAvailable[currentItem]);
        }
    }

    private void SetHighlight(){
        for(int i = 0; i < itemTabs.Length; i++){
            itemTabs[i].GetNode<TextureRect>("Highlight").Visible = i == currentItem;
        }
    }
    private void SetDescriptionBox(Item descItem){
        itemIcon.Texture = descItem.GetIcon();
        titleLineLabel.Text = "[b]" + descItem.GetDisplayName() + "[/b]\n[center]Item - Consumable[/center]";
        descriptionLabel.Text = descItem.GetRulesText() + "\n\n[i]" + descItem.GetFlavorText() + "[/i]";
    }

    public override PlayerAbility HandleInput(PlayerInput input, PlayerCombatant character, Battle caller, BattleGUI parentGUI){
        switch(input){
            case PlayerInput.Up:
                if(currentItem > 0) currentItem--;
                break;
            case PlayerInput.Down:
                if(currentItem < itemsAvailable.Length - 1) currentItem++;
                break;
            case PlayerInput.Back:
                parentGUI.ChangeMenu(0, character); //Goes back to top menu
                break;
            case PlayerInput.Select:
                character.GetNode<UseItemAction>("Use Item").SetItemAndDetails(itemsAvailable[currentItem]);
                NewTargetingMenu tMenu = (NewTargetingMenu) parentGUI.menus[5];
                tMenu.SetAbilityForTargeting(character.GetNode<UseItemAction>("Use Item"), character, caller, parentGUI);
                parentGUI.ChangeMenu(5, character);
                break;
        }
        if(input != PlayerInput.None){
            SetHighlight();
        }
        return null;
    }
}

    
