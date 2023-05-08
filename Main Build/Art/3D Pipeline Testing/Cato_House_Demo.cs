using Godot;
using System;

public partial class Cato_House_Demo : Node3D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Process(double delta)
    {
        if(Input.IsActionJustPressed("ui_accept")){
            AnimationPlayer player = (AnimationPlayer) GetNode("AnimationPlayer");
            player.Play("Dollhouse Up");
        }
    }
}
