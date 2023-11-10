using Godot;
using System;
using static GameplayUtilities;
public partial class PauseMenuGUI : CanvasLayer
{
	public enum PauseMenuMode{
		SelectACharacter
	}
	PauseMenuMode currentMode;
	[Export]
	ItemMenu itemSubMenu;

	[Export]
	ReadoutContainer readouts;
	[Export]
	Button startNightButton;
	int selectX, selectY;

	[Signal]
	public delegate void CharacterSelectedInPauseMenuEventHandler(StringName name);

	public void Open()
	{
		itemSubMenu.Visible = true;
		itemSubMenu.OnOpenInsidePauseMenu();
		//Populate Readouts
		var party = this.GetNode<GameMaster>("/root/GameMaster").LoadPartyData();
		selectX = 0;
		selectY = 0;
		readouts.SetReadoutsPauseMenu(party);
	}

	public override void _Process(double delta){
		startNightButton.Disabled = this.GetNode<GameMaster>("/root/GameMaster").GetCurrentTU() != 0;
	}

	//returns true if we should switch back to the earlier gameplaymode
	public bool HandleInput(PlayerInput input){
		if(input == PlayerInput.Start || input == PlayerInput.Back){
			return true;	
		}
		switch(currentMode){
			case PauseMenuMode.SelectACharacter:
				MoveSelection(0,0, 0, readouts.Length(), input);
				readouts.SetSelectedByIndex(selectY);
				if(input == PlayerInput.Select){
					EmitSignal(PauseMenuGUI.SignalName.CharacterSelectedInPauseMenu,
					readouts.GetCharacterNameAtIndex(selectY));
					return true;
				}
				break;
		}
		
		itemSubMenu.HandleInputPauseMenu(input);	
		
		return false;
	}

	public void OnPressQuit(){
		GetTree().Quit();
	}

	public void StartNight(){
		if(startNightButton.Disabled != true){
			this.GetNode<CityState>("/root/CityState").GetCity().StartNight();
		}
	}
	public void Close()
	{

	}

	public void SetMode(PauseMenuMode mode){
		currentMode = mode;
		selectX = 0;
		selectY = 0;
	}

	public void MoveSelection(int minX, int maxX, int minY, int maxY, PlayerInput input){
		switch(input){
			case PlayerInput.Up : if(selectY < maxY) selectY++; break;
			case PlayerInput.Down : if(selectY > minY) selectY--; break;
			case PlayerInput.Left : if(selectX > minX) selectY++; break;
			case PlayerInput.Right : if(selectX < maxX) selectY--; break;
		}
	}
}
