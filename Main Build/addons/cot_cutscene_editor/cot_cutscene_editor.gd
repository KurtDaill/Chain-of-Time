@tool
extends EditorPlugin

const MainPanel = preload("res://addons/cot_cutscene_editor/cutscene_main_screen.tscn")

var main_panel_instance

func _enter_tree():
	# Initialization of the plugin goes here.
	add_custom_type("Actor", "Node3D", preload("res://addons/cot_cutscene_editor/scripts/Actor.cs"), preload("res://addons/cot_cutscene_editor/ActorIcon.png"));
	add_custom_type("DialogueBox", "Control", preload("res://addons/cot_cutscene_editor/scripts/CutsceneDialogueBox.cs"), preload("res://addons/cot_cutscene_editor/DialogueBubbleIcon.png"));
	add_custom_type("Cutscene", "Node3D", preload("res://addons/cot_cutscene_editor/scripts/CutsceneDirector.cs"), preload("res://addons/cot_cutscene_editor/CutsceneDirectorIcon.png"));
	add_custom_type("ResponseBox", "VBoxContainer", preload("res://addons/cot_cutscene_editor/scripts/CutsceneResponseBox.cs"), preload("res://addons/cot_cutscene_editor/CutsceneResponseBoxIcon.png"));
	add_custom_type("Shot", "Camera3D", preload("res://addons/cot_cutscene_editor/scripts/CutsceneShot.cs"), preload("res://addons/cot_cutscene_editor/CutsceneShotIcon.png"));
	
	main_panel_instance = MainPanel.instantiate()
    # Add the main panel to the editor's main viewport.
	get_editor_interface().get_editor_main_screen().add_child(main_panel_instance)
    # Hide the main panel. Very much required.
	_make_visible(false)
	
func _exit_tree():
	# Clean-up of the plugin goes here.
	remove_custom_type("Actor");
	remove_custom_type("DialogueBox");
	remove_custom_type("Cutscene");
	remove_custom_type("Shot");
	if main_panel_instance:
		main_panel_instance.queue_free()

#func _process(delta):
	#await RenderingServer.frame_post_draw
	#if(get_editor_interface().get_edited_scene_root().name == "Cutscene Main View Test Scene"):
		#main_panel_instance.get_node("Main Editor/Cutscene Camera View").texture.set_image(get_editor_interface().get_edited_scene_root().get_node("Cutscene Preview").get_texture().get_image())
		
	#for node in get_editor_interface().get_edited_scene_root().find_children("*", "CutsceneDirector"):
		#node.GetCutsceneCamera()

func _has_main_screen():
	return false

func _make_visible(visible):
	if main_panel_instance:
		main_panel_instance.visible = visible

func _get_plugin_name():
	return "Cutscene"

func _get_plugin_icon():
	return get_editor_interface().get_base_control().get_theme_icon("Node", "EditorIcons")
	
