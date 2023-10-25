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
	[Export]
	Node3D spawnPointDirectory;

	GameMaster gm;

	public override void _Ready(){
		gm = GetNode<GameMaster>("/root/GameMaster");
		gm.SetMode(startingMode);
		if(gm.GetSpawnPoint() != "" && startingMode is ExploreMode){
			if(spawnPointDirectory.GetNode<Node3D>(gm.GetSpawnPoint()) != null){
			((ExploreMode)startingMode).SetExplorePlayerPosition(spawnPointDirectory.GetNode<Node3D>(gm.GetSpawnPoint()).GlobalPosition);
			gm.ClearSpawnPoint();
			}else{
				throw new ArgumentException("spawnPoint listed is not recognized in target scene! Spawn Point was:" + gm.GetSpawnPoint());
			}
		}
	}
	public void StopMusic(){
		bgMusic.Stop();
	}

	public void StartMusic(){
		bgMusic.Play();
	}
}


