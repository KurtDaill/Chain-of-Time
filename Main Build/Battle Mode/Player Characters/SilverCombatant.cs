using System;
using Godot;

public partial class SilverCombatant : PlayerCombatant{
    public override void _Ready()
	{
		base._Ready();
        name = "Silver";
	}
}