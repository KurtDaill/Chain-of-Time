using Godot;
using System;
using static GameplayUtilities;
public partial class PauseMenuGUI : CanvasLayer
{
	public enum PauseMenuMode{
		SelectACharacter,
		NormalPause
	}
	PauseMenuMode currentMode;
	[Export]
	ItemMenu itemSubMenu;

	[Export]
	ReadoutContainerPauseMenu readouts;
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
				MoveSelectionInvertY(0,0, 0, readouts.Length(), input);
				readouts.SetSelectedByIndex(selectY);
				if(input == PlayerInput.Select){
					EmitSignal(PauseMenuGUI.SignalName.CharacterSelectedInPauseMenu,
					readouts.GetCharacterNameAtIndex(selectY));
					return true;
				}
				break;
			case PauseMenuMode.NormalPause: 
				//Include Stuff for the Default Pause Menu Here
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
			if(this.GetNode<SceneConfig>("/root/SceneConfig").IsCityScene()){
				//If we're in a city scene, we can just switch where we're at.
				this.GetNode<CityState>("/root/CityState").GetCity().StartNight();
			}else{
				//If we're not in a city scene, we'll have to manually load the main scene.
				//This function call forces the next load of the city to go to Night Defense
				this.GetNode<CityState>("/root/CityState").SetNextCityLoadToBeNightDefesnse();
				GetTree().ChangeSceneToPacked(GetNode<GameMaster>("/root/GameMaster").GetMainScenePacked());
			}
		}
	}
	public void Close()
	{
		readouts.SetSelectedByIndex(-1);
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
	public void MoveSelectionInvertY(int minX, int maxX, int minY, int maxY, PlayerInput input){
		switch(input){
			case PlayerInput.Down : if(selectY < maxY) selectY++; break;
			case PlayerInput.Up : if(selectY > minY) selectY--; break;
			case PlayerInput.Left : if(selectX > minX) selectY++; break;
			case PlayerInput.Right : if(selectX < maxX) selectY--; break;
		}
	}
}
