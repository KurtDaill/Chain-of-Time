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
      //Check if any of them are an Interactable's Interact Areas

      //Run the Interact() function, if we get true, that means it was a valid interact and we break;
      //If Interact() returns false, we keep trying to find other Interactables

      //At the end of Process, we delete this Area2D: we don't need it anymore

    }
    
    public void _on_InteractFocusBox_area_entered(Area2D area){
      var temp = area.GetParent().GetType();
      if(temp == typeof(Interactable)){
        Interactable target = (Interactable) area.GetParent();
        if(target.Interact()){
         QueueFree();
        }
      }
    }

    public void _on_Timer_timeout(){
      QueueFree();
    }
}
