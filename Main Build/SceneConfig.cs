using Godot;
using System;
using System.Threading.Tasks;
using static GameplayUtilities;
public partial class SceneConfig : Node
{
	[Export]
	GameplayMode startingMode;

	[Export]
	AudioStreamPlayer bgMusic;

	GameMaster gm;

	public override void _Ready(){
		gm = GetNode<GameMaster>("/root/GameMaster");
		gm.SetMode(startingMode);
		startingMode.StartUp();
	}
	public void StopMusic(){
		bgMusic.Stop();
	}

	public void StartMusic(){
		bgMusic.Play();
	}
}


