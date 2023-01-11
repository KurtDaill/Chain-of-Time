using Godot;
using System;
using static PMCharacterUtilities;

public partial class StatusEffectParticles : Node3D
{
	[Export]
	private BodyRegions spawnRegion = BodyRegions.Head;

	//Unique Identifier used by the core status effect to find and modify this particle effect if needed

	public BodyRegions GetSpawnRegion(){
		return spawnRegion;
	}

	public void End(){
		this.QueueFree();
	}
}
