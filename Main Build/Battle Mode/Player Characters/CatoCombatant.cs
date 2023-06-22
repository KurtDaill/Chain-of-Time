using System;
using Godot;

public partial class CatoCombatant : PlayerCombatant{
    public override void _Ready()
	{
		base._Ready();
        name = "Cato";
	}
}