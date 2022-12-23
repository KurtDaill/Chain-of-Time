using Godot;
using System;

public partial class Interactable : StaticBody2D
{
    //Returns whether or not a valid interaction has occured
   public bool Interact(){
       this.Visible = false;
       return true;
   }
}
