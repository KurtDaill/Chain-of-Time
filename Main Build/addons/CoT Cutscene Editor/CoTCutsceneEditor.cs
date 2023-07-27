#if TOOLS
using Godot;
using System;

[Tool]
public partial class CoTCutsceneEditor : EditorPlugin
{
	public override void _EnterTree()
	{
		// Initialization of the plugin goes here.
		AddCustomType("Actor", "Node3D", GD.Load<Script>("res://addons/CoT Cutscene Editor/Actor.cs"), GD.Load<Texture2D>("res://icon.png"));
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
		RemoveCustomType("Actor");
	}
}
#endif
