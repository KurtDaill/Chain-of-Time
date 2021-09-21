using Godot;
using System;

public class TestResolutionSwap : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    Viewport vp;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready(){     
            vp = GetTree().Root;
    }

    public override void _Process(float delta){
        if(Input.IsActionJustPressed("ui_accept")){
            Console.WriteLine("Hit Enter");
            if(vp.Size == new Vector2(320, 180)){
                vp.Size = new Vector2(640, 360);
                //vp.SetSizeOverride(true, new Vector2(640,360), Vector2.Zero);
            }else{
                vp.Size = new Vector2(320, 180);
                //vp.SetSizeOverride(false, Vector2.Zero, Vector2.Zero);
            }
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
