using Godot;
using System;

public partial class CronoScene : CSGCombiner3D
{
	//Includes Characters, Cutscenes, encounters, and other things that have to be despawned/spawned when changing times
	[Export]
	Godot.Collections.Array<NodePath> childModulePath;
	Godot.Collections.Array<Node3D> childModules = new Godot.Collections.Array<Node3D>(); 
	public override void _Ready(){
		foreach(NodePath path in childModulePath){
			childModules.Add(GetNode<Node3D>(path));
		}
	}

	public void HideChildModules(){
		foreach(Node3D child in childModules){
			child.Visible = false;
		}
	}

	public void ShowChildModules(){
		foreach(Node3D child in childModules){
			child.Visible = true;
		}
	}
}
