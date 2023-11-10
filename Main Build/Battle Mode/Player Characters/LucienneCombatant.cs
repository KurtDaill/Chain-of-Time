using System;
using Godot;

public partial class LucienneCombatant : PlayerCombatant{
	public override void _Ready()
	{
		base._Ready();
		name = "Lucienne";
	}
}
