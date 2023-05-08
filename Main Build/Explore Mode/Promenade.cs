using Godot;
using System;

public partial class Promenade : Area3D
{
	[Export]
	public int promenadeIndex = 0;
	public override void _Ready(){
		this.AddToGroup("Promenade");
	}

	public int GetPromenadeIndex(){
		return promenadeIndex;
	}
}
