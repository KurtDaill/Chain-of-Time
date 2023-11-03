using Godot;
using System;
using static GameplayUtilities;
public partial class PauseMenuGUI : CanvasLayer
{

	[Export]
	ItemMenu itemSubMenu;

	[Export]
	ReadoutContainer readouts;
	[Export]
	Button startNightButton;
	public void Open()
	{
		itemSubMenu.Visible = true;
		itemSubMenu.OnOpenInsidePauseMenu();
		//Populate Readouts
		var party = this.GetNode<GameMaster>("/root/GameMaster").LoadPartyData();
		readouts.SetReadoutsPauseMenu(party);
	}

	public override void _Process(double delta){
		startNightButton.Disabled = this.GetNode<GameMaster>("/root/GameMaster").GetCurrentTU() != 0;
	}

	//returns true if we should switch back to the earlier gameplaymode
	public bool HandleInput(PlayerInput input){
		if(input == PlayerInput.Start){
			return true;	
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
}
