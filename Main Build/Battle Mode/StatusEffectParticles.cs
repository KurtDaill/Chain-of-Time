using Godot;
using System;
//using static PMCharacterUtilities;

public partial class StatusEffectParticles : Node3D
{
	[Export]
	//private BodyRegions spawnRegion = BodyRegions.Head;
	AnimationPlayer animPlay;

	//Unique Identifier used by the core status effect to find and modify this particle effect if needed
	public override void _Ready(){
		animPlay = this.GetNode<AnimationPlayer>("AnimationPlayer");
	}
	//public BodyRegions GetSpawnRegion(){
	//	return spawnRegion;
	//}

	public void End(){
		this.QueueFree();
	}

	public void StartUpkeep(){
		animPlay.Play("Upkeep");
	}
	
	public void Expire(){
		animPlay.Play("Expire");
	}
}
