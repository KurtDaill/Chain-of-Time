@tool
extends EditorPlugin


func _enter_tree():
	# Initialization of the plugin goes here.
	add_custom_type("Actor", "Node3D", preload("res://addons/cot_cutscene_editor/scripts/Actor.cs"), preload("res://addons/cot_cutscene_editor/ActorIcon.png"));
	add_custom_type("DialogueBox", "Node3D", preload("res://addons/cot_cutscene_editor/scripts/CutsceneDialogueBox.cs"), preload("res://addons/cot_cutscene_editor/DialogueBubbleIcon.png"));
	add_custom_type("Cutscene", "Node3D", preload("res://addons/cot_cutscene_editor/scripts/CutsceneDirector.cs"), preload("res://addons/cot_cutscene_editor/CutsceneDirectorIcon.png"));
	add_custom_type("ResponseBox", "Node3D", preload("res://addons/cot_cutscene_editor/scripts/CutsceneResponseBox.cs"), preload("res://addons/cot_cutscene_editor/CutsceneResponseBoxIcon.png"));
	add_custom_type("Shot", "Camera3D", preload("res://addons/cot_cutscene_editor/scripts/CutsceneShot.cs"), preload("res://addons/cot_cutscene_editor/CutsceneShotIcon.png"));

func _exit_tree():
	# Clean-up of the plugin goes here.
	remove_custom_type("Actor");
	remove_custom_type("DialogueBox");
	remove_custom_type("Cutscene");
	remove_custom_type("Shot");
