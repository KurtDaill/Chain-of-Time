using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BattlePointMap : Node3D
{
	//Orrientation matters for Battle Points : TODO have a better way of reminding folks of that or just integrate it into a Grid 3D solution?
	List<Marker3D> battlePointsInCity;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(this.GetChildren().Any(x => x is not Marker3D)) throw new Exception("You can't have non Marker3D nodes parented to Battle Point Map.");
		battlePointsInCity = new List<Marker3D>();
		foreach(Node node in this.GetChildren()){
			if(node is Marker3D) battlePointsInCity.Add((Marker3D)node);
		}
	}

	public Marker3D GetClosestBattlePoint(Vector3 position){
		float lowestDiff = (battlePointsInCity[0].GlobalPosition - position).Length();
		Marker3D result = battlePointsInCity[0];
		foreach(Marker3D mark in battlePointsInCity){
			float diff = (mark.GlobalPosition - position).Length();
			if(diff < lowestDiff){
				lowestDiff = diff;
				result = mark;
			}
		}
		return result;
	}
}
