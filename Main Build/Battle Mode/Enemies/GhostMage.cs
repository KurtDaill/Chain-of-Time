using Godot;
using System;

public partial class GhostMage : EnemyCombatant
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	private EnemyAbility attack;
	[Export]
	private EnemyAbility confusion;
	[Export]
	private EnemyAbility fireBlast;
	public override void _Ready()
	{
		
        name = "Ghost Mage";
        base._Ready();
	}

}
