using Godot;
using System;

public partial class Encounter : Area3D
{
	[Export]
	public bool enabled;
	[Export]
	PMBattle battle;
	[Export]
	Sprite3D sprite;
	[Export]
	CutsceneDirector postFightCutscene = null;

	ExplorePlayer triggeringPlayer;

	private bool battleArmed = true;

	public void StartEncounter(ExplorePlayer player){
		if(!enabled) return;
		if(battleArmed){
			battle.StartBattleFromExplore();
			triggeringPlayer = player;
			triggeringPlayer.SetActive(false);
			sprite.Visible = false;
			GetNode<CameraManager>("/root/CameraManager").SwitchCamera(battle.GetBattleCamera());
			battleArmed = false;
		}
	}

	public void FinishEncounter(){
		battle.QueueFree();
		battleArmed = false;
		if(postFightCutscene != null){
			postFightCutscene.StartCutscene();
		}else{
			triggeringPlayer.SetActive(true);
			GetNode<CameraManager>("/root/CameraManager").SwitchCamera(triggeringPlayer.exploreCamera);
		}
	} 
}
