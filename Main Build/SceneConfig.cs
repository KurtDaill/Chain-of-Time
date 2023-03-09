using Godot;
using System;

public partial class SceneConfig : Node
{
	[Export]
	NodePath startCustscene;
	[Export]
	NodePath startBattle;
	[Export]
	Camera3D camera;
	[Export]
	ExplorePlayer ePlayer;
	[Export(PropertyHint.Enum)]
	StartingMode beginIn;

	private enum StartingMode{
		Explore,
		Cutscene,
		Battle
	};

	public override void _Ready(){
		//this.GetNode<CameraManager>("/root/CameraManager").SwitchCamera(camera);
		ePlayer.SetActive(false);
		switch(beginIn){
			case StartingMode.Explore:
				ePlayer.SetActive(true);
				break;
			case StartingMode.Cutscene:
				GetNode<CutsceneDirector>(startCustscene).StartCutscene();
				break;
			case StartingMode.Battle:
				GetNode<Encounter>(startBattle).StartEncounter(ePlayer);
				break;
		}
	}

	public ExplorePlayer GetExplorePlayer(){
		return ePlayer;
	}
}


