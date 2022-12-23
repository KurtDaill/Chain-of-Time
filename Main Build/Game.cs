using Godot;
using System;

public partial class Game : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Signal]
    public delegate void PlayerWakeEventHandler();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    public override void _Process(double delta){
    }
    public void wakeUpPlayer(){
        EmitSignal(nameof(PlayerWakeEventHandler));
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(double delta)
//  {
//      
//  }
}
