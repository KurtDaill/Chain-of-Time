using Godot;
using System;

public class TextboxSpawnTest : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    //public override void _Ready()
    //{
        
    //}

    public override void _Process(float delta){
        if(Input.IsActionPressed("ui_accept")){
            SpawnTextbox("Sup Nerd.");
        }
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void SpawnTextbox(String text){
        var scene = GD.Load<PackedScene>("res://GUI/BasicTextBox.tscn");
        var instance = scene.Instance();
        AddChild(instance);
        RichTextLabel label = (RichTextLabel) instance.GetChild(0);
        label.Text = text;
    }
}
