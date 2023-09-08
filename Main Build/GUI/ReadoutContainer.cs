using Godot;
using System;
using static BattleUtilities;
public partial class ReadoutContainer : BoxContainer
{     

	[Signal]
	public delegate void ReadyToPopulateReadoutsEventHandler(); 
	[Export]
	Godot.Collections.Array<PlayerCharacterReadout> readouts;
	[Export]
	TopMenu topMenu;

	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(double delta)
//  {
//      
//  }
	
	public void SetSelectedCharacter(PlayerCombatant com){
		for(int i = 0; i < readouts.Count; i++){
			if(readouts[i].character == com){
				readouts[i].Select();
				PositionTopMenu(i);
			}else{
				readouts[i].Deselect();
			}
		}
	}
	public void Reorder(){ //TODO Actually Make this work with any number of players
		var readouts = this.GetChildren();
		foreach(Node element in readouts){
			if(element != null && element is PlayerCharacterReadout){
				var read = (PlayerCharacterReadout) element; //TODO Better handling of...all of this...
				if(read.character == null) continue;
				switch(read.character.GetPosition().GetRank()){
					case BattleRank.HeroFront :
						MoveChild(read, 1);
						break;
					case BattleRank.HeroMid :
						MoveChild(read, 0);
						break;
					case BattleRank.HeroBack :
						MoveChild(read, 0);
						break;
				}
			}
			//else readouts.Remove(element);
		}
	}

	private void PositionTopMenu(int currentCharacter){
		this.MoveChild(topMenu, (readouts.Count - currentCharacter));
	}

	public void SetReadouts(PlayerCombatant[] playerCombatants){
		foreach(Node child in this.GetChildren()){
			this.RemoveChild(child);
		}
		this.AddChild(topMenu);
		foreach(PlayerCombatant pCom in playerCombatants){
			this.AddChild(pCom.GetReadoutInstanced());			
		}
	}
}
