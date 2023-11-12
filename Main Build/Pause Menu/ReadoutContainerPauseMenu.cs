using System;
using Godot;

public partial class ReadoutContainerPauseMenu : BoxContainer{
    Godot.Collections.Array<PlayerCharacterReadout> readouts;

	public void SetReadoutsPauseMenu(PlayerCombatant[] playerCombatants){
		foreach(Node child in this.GetChildren()){
			this.RemoveChild(child);
		}
		readouts = new Godot.Collections.Array<PlayerCharacterReadout>();
		foreach(PlayerCombatant pCom in playerCombatants){
			PlayerCharacterReadoutPauseMenu readout = pCom.GetReadoutInstacedPauseMenu();
			this.AddChild(readout);
			readouts.Add(readout);			
		}
	}

	public void SetSelectedByIndex(int index){
		for(int i = 0; i < readouts.Count; i++){
			if(i == index){
				readouts[i].Select();
				//PositionTopMenu(i);
			}else{
				readouts[i].Deselect();
			}
		}
	}
	
	public string GetCharacterNameAtIndex(int index){
		return readouts[index].character.GetName();
	}

	public int Length(){
		return readouts.Count;
	}
}