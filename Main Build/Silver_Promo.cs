using Godot;
using System;

public class Silver_Promo : AnimatedSprite3D
{
    [Export]
    NodePath poly;
    Combatant polymorph;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        polymorph = (Combatant) GetNode(poly);
    }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
    if(Input.IsActionJustPressed("ui_accept")){
        this.Playing = true;
    }
      if(this.Frame == 6){
        polymorph.animSM.Travel("Hit React");
      }

    if(this.Frame == 23){
      this.Playing = false;
    }
  }
}
