using Godot;
using System;

public partial class InContextSpriteTest : Node3D
{
	[Export]
	int startingIndex = 0;
	[Export]
	private Godot.Environment[] enviornments;
	[Export]
	private Godot.NodePath[] stagesInEditor = new NodePath[9];
	private Godot.Node3D[] stages;
	[Export]
	public WorldEnvironment env;

	public override void _Ready(){
		stages = new Node3D[stagesInEditor.Length];
		for(int i = 0; i < stagesInEditor.Length; i++){
			stages[i] = (Node3D) GetNode(stagesInEditor[i]);
		}
		SwitchTestScene(startingIndex);
	}
	public override void _Process(double delta){
		if(Input.IsActionJustPressed("debug_0")){
			SwitchTestScene(0);
		}
		if(Input.IsActionJustPressed("debug_1")){
			SwitchTestScene(1);
		}
		if(Input.IsActionJustPressed("debug_2")){
			SwitchTestScene(2);
		}
		if(Input.IsActionJustPressed("debug_3")){
			SwitchTestScene(3);
		}
		if(Input.IsActionJustPressed("debug_4")){
			SwitchTestScene(4);
		}
		if(Input.IsActionJustPressed("debug_5")){
			SwitchTestScene(5);
		}
		if(Input.IsActionJustPressed("debug_6")){
			SwitchTestScene(6);
		}
		if(Input.IsActionJustPressed("debug_7")){
			SwitchTestScene(7);
		}
		if(Input.IsActionJustPressed("debug_8")){
			SwitchTestScene(8);
		}
		if(Input.IsActionJustPressed("debug_9")){
			SwitchTestScene(9);
		}
	}

	public void SwitchTestScene(int index){
		for(int i = 0; i < stages.Length; i++){
			if(i == index){
				stages[i].Visible = true;
				env.Environment = enviornments[i];
			}
			else{
				stages[i].Visible = false;
			}
		}
	}
}
