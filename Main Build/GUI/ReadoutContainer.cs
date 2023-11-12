using Godot;
using System;
using static BattleUtilities;
public partial class ReadoutContainer : BoxContainer
{     

	[Signal]
	public delegate void ReadyToPopulateReadoutsEventHandler(); 

	//The array stores the character position inately in the readout's place in the array. The data is ordered as [Rank,Lane]
	PlayerCharacterReadout[,] myReadouts;
	int numberOfActiveReadouts;
	[Export]
	TopMenu topMenu;

	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		myReadouts = new PlayerCharacterReadout[3,3];
	}

	public void SetSelectedCharacter(PlayerCombatant com){
		for(int i = 0; i < 3; i++){
			for(int j = 0; j < 3; j++){
				if(myReadouts[i,j] != null){
					if(myReadouts[i,j].character == com){
						myReadouts[i,j].Select();
						Reorder(myReadouts[i,j].GetIndex());
					}else{
						myReadouts[i,j].Deselect();
					}
				}
			}
		}
	}

	public void SetCharacterReadouts(PlayerCombatant[] playerCombatants){
		numberOfActiveReadouts = 0;
		foreach(PlayerCharacterReadout readout in myReadouts){
			if(IsInstanceValid(readout) && readout != null){
				readout.Free();
			}
		}
		myReadouts = new PlayerCharacterReadout[3,3];
		foreach(PlayerCombatant com in playerCombatants){
			myReadouts[(int)com.GetPosition().GetRank(), (int)com.GetPosition().GetLane()] = com.GetReadoutInstanced();
			this.AddChild(myReadouts[(int)com.GetPosition().GetRank(), (int)com.GetPosition().GetLane()]);
			numberOfActiveReadouts++;	
		}
		Reorder();
	}
	public void Reorder(int currentCharacter = -1){ //TODO Actually Make this work with any number of players
		int index = 0;
		for(int i = 0; i < 3; i++){
			for(int j = 0; j < 3; j++){
				if(myReadouts[i,j] != null){
					this.MoveChild(myReadouts[i,j], index);
					index++;
				}
			}
		}
		if(currentCharacter != -1) PositionTopMenu(currentCharacter);
	}

	private void PositionTopMenu(int currentCharacter){
		this.MoveChild(topMenu, currentCharacter + 1);
	}
}
