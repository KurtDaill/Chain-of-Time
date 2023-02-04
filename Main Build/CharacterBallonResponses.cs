using Godot;
using System;

public partial class CharacterBallonResponses : VBoxContainer
{
	//[Export]
	public Godot.Collections.Array<RichTextLabel> responses;
	public override void _Ready()
	{
		responses = new Godot.Collections.Array<RichTextLabel>();
		foreach(Node child in this.GetChildren()){
			if(child is RichTextLabel){
				responses.Add((RichTextLabel)child);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
