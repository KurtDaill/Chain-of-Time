using Godot;
using System;

public partial class CronoScene : CSGCombiner3D
{
	//Includes Characters, Cutscenes, encounters, and other things that have to be despawned/spawned when changing times
	[Export]
	bool beginEnabled;
	[Export]
	Godot.Collections.Array<NodePath> childModulePath;
	Godot.Collections.Array<Node3D> childModules = new Godot.Collections.Array<Node3D>(); 
	public override void _Ready(){
		foreach(NodePath path in childModulePath){
			childModules.Add(GetNode<Node3D>(path));
		}
		if(beginEnabled){
			ShowChildModules();
		}else{
			HideChildModules();
		}
	}

	public void HideChildModules(){
		foreach(Node3D child in childModules){
			if(child != null) child.Visible = false;
			if(child is Encounter){
				((Encounter)child).enabled = false;
			}
			if(child is CutsceneDirector){
				((CutsceneDirector)child).enabled = false;
			}
			if(child is ExploreNPC){
				((ExploreNPC)child).enabled = false;
			}
		}
	}

	public void ShowChildModules(){
		foreach(Node3D child in childModules){
			child.Visible = true;
			if(child is Encounter){
				((Encounter)child).enabled = true;
			}
			if(child is CutsceneDirector){
				((CutsceneDirector)child).enabled = true;
			}
			if(child is ExploreNPC){
				((ExploreNPC)child).enabled = true;
			}
		}
	}
}
