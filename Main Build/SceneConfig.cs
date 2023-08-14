using Godot;
using System;
using System.Threading.Tasks;
using static GameplayUtilities;
public partial class SceneConfig : Node
{
	[Export]
	NodePath startCustscene;
	[Export]
	Battle startBattle;
	[Export]
	Camera3D camera;
	[Export]
	ExplorePlayer ePlayer;
	[Export(PropertyHint.Enum)]
	StartingMode beginIn;
	[Export]
	AudioStreamPlayer bgMusic;

	GameMaster gm;

	private enum StartingMode{
		Explore,
		Cutscene,
		Battle
	};

	public override void _Ready(){
		gm = this.GetNode<GameMaster>("/root/GameMaster");
		switch(beginIn){
			case StartingMode.Battle:
				gm.SetMode(startBattle); break;
		}
	}

	public ExplorePlayer GetExplorePlayer(){
		return ePlayer;
	}

	public void StopMusic(){
		bgMusic.Stop();
	}

	public void StartMusic(){
		bgMusic.Play();
	}
}


