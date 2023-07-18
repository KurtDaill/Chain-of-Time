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
	VBoxContainer topMenu;

	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EmitSignal(nameof(ReadyToPopulateReadouts), this);
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(double delta)
//  {
//      
//  }
	
	public void SetSelectedCharacter(int index){
		for(int i = 0; i < readouts.Count; i++){
			if(i == index){
				readouts[i].Select();
				PositionTopMenu(i);
			}else{
				readouts[i].Deselect();
			}
		}
	}
	public void Reorder(){
		var readouts = this.GetChildren();
		foreach(Node element in readouts){
			if(element != null && element is PlayerCharacterReadout){
				var read = (PlayerCharacterReadout) element;
				switch(read.character.GetPosition().GetRank()){
					case BattleRank.HeroFront :
						MoveChild(read, 0);
						break;
					case BattleRank.HeroMid :
						MoveChild(read, 1);
						break;
					case BattleRank.HeroBack :
						MoveChild(read, 2);
						break;
				}
			}
			else readouts.Remove(element);
		}
	}

	private void PositionTopMenu(int currentCharacter){
		this.MoveChild(topMenu, (readouts.Count - currentCharacter));
	}
}
