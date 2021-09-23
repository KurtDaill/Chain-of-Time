using Godot;
using System;

public class PlayerInteractCheck : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }


    public override void _Process(float delta)
    {
      //Get all overlapping Area2D's
      //Check if any of them are Item's Interact Areas

      //Run the Interact() function, if we get true, that means it was a valid interact and we break;
      //If Interact() returns false, we keep trying to find other items

      //At the end of Process, we delete this Area2D: we don't need it anymore

    }
}
