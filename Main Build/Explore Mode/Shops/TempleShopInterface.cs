using Godot;
using System;

public partial class TempleShopInterface : ShopInterface 
{
	protected override GameplayMode ProcessButtonPress(string activateString){
        switch(activateString){
            case "HealCharacter":
                if(gm.GetCurrentTU() >= 1){
                    //Set the Pause Menu to be in Select Mode
                    this.GetNode<PauseMenu>("/root/PauseMenu").SetMenuTypeOnTransitionIn("Select-A-Character");
                    //Link OnCharacterSelectedForHealing (which wil take it from here)
                    this.GetNode<PauseMenu>("/root/PauseMenu").GetGUI().CharacterSelectedInPauseMenu += OnCharacterSelectedForHealing;
                    this.GetNode<PauseMenu>("/root/PauseMenu").GetGUI().PlayerClosesMenu += QuitOutOfHealing;
                    //Set the Game to be in the Pause Menu (In the Select Mode)
                    this.GetNode<GameMaster>("/root/GameMaster").SetMode(this.GetNode<PauseMenu>("/root/PauseMenu"));
                }
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
        this.GetNode<PauseMenu>("/root/PauseMenu").GetGUI().CharacterSelectedInPauseMenu -= OnCharacterSelectedForHealing;
        this.GetNode<PauseMenu>("/root/PauseMenu").GetGUI().PlayerClosesMenu -= QuitOutOfHealing;
        gm.RestoreCharacterHP(characterSelected);
        gm.SpendTU(1);
        descritpionTextBox.Text = characterSelected + " Healed.";   
    }
    private void QuitOutOfHealing(){
        this.GetNode<PauseMenu>("/root/PauseMenu").GetGUI().CharacterSelectedInPauseMenu -= OnCharacterSelectedForHealing;
        this.GetNode<PauseMenu>("/root/PauseMenu").GetGUI().PlayerClosesMenu -= QuitOutOfHealing;
    }
}
