using Godot;
using System;
using static PMCharacterUtilities;

public partial class StatusEffectParticles : Node
{
	[Export]
	private BodyRegions spawnRegion = BodyRegions.Head;

	public BodyRegions GetSpawnRegion(){
		return spawnRegion;
	}
}
