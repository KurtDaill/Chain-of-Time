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
		//if(!enabled) return;
		if(battleArmed){
			battleArmed = false;
			battle.StartBattleFromExplore();
			triggeringPlayer = player;
			triggeringPlayer.SetActive(false);
			if(sprite != null) sprite.Visible = false;
			GetNode<CameraManager>("/root/CameraManager").SwitchCamera(battle.GetBattleCamera());
			battleArmed = false;
			this.GetNode<SceneConfig>("/root/Node3D").StopMusic();
		}
	}

	public void FinishEncounter(){
		battle.QueueFree();
		battleArmed = false;
		this.GetNode<SceneConfig>("/root/Node3D").StartMusic();
		if(postFightCutscene != null){
			postFightCutscene.StartCutscene();
		}else{
			triggeringPlayer.SetActive(true);
			GetNode<CameraManager>("/root/CameraManager").SwitchCamera(triggeringPlayer.exploreCamera);
		}
	} 
}
