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
	[Export]
	Marker3D citySpawnPoint;
	[Export]
	bool cityScene = false;
	[Export]
	NightDefense myNightDefenseMode;

	GameMaster gm;
	CityState cs;

	public override void _Ready(){
		gm = GetNode<GameMaster>("/root/GameMaster");
		gm.SetMode(startingMode);
		cs = GetNode<CityState>("/root/CityState");
		if(cityScene){
			if(citySpawnPoint == null) throw new ArgumentException("There has to be a citySpawnPoint if you want to spawn the city!");
			cs.InstantiateSavedCityAtPoint(citySpawnPoint);
			spawnPointDirectory = cs.GetCity().GetSpawnPointDirectory();
		}
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

	public NightDefense GetMyNightDefenseMode(){
		return myNightDefenseMode;
	}

	public void TransitionToNewScene(string spawnPoint, string targetScene){
		if(cityScene){
			this.GetNode<CityState>("/root/CityState").SaveCity(this.GetNode<CityState>("/root/CityState").GetCity());
			this.GetNode<CityState>("/root/CityState").DespawnCity();
		}
        if(spawnPoint != "") this.GetNode<GameMaster>("/root/GameMaster").SetSpawnPoint(spawnPoint);
        GetTree().ChangeSceneToFile(targetScene);
	}
}


