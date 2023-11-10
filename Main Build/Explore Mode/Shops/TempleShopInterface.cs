using Godot;
using System;

public partial class TempleShopInterface : ShopInterface 
{
	protected override GameplayMode ProcessButtonPress(string activateString){
        switch(activateString){
            case "HealCharacter":
                //Set the Pause Menu to be in Select Mode
                this.GetNode<PauseMenu>("/root/PauseMenu").SetMenuTypeOnTransitionIn("Select-A-Characer");
                //Link OnCharacterSelectedForHealing (which wil take it from here)
                this.GetNode<PauseMenu>("/root/PauseMenu").GetGUI().CharacterSelectedInPauseMenu += OnCharacterSelectedForHealing;
                //Set the Game to be in the Pause Menu (In the Select Mode)
                this.GetNode<GameMaster>("/root/GameMaster").SetMode(this.GetNode<PauseMenu>("/root/PauseMenu"));
                return null;
            case "HealParty":
                if(gm.GetCurrentTU() >= 2){
						gm.RestorePartyHP();
                        gm.SpendTU(2);
                        descritpionTextBox.Text = "Healed.";
                }
                return null;
            default: throw new ArgumentException("Button Activation String Error. Button Activated String: " + activateString + " not recognized by Shop Interface: " + Name + ".");
        }
    }

    private void OnCharacterSelectedForHealing(StringName characterSelected){
        gm.RestoreCharacterHP(characterSelected);
        descritpionTextBox.Text = characterSelected + " Healed.";
    }
}
