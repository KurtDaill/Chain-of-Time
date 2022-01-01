using Godot;
using System;

public class BattlePlayer : Combatant
{

    public override void _Ready()
    {
        //Finds Relevant Children
        Godot.Collections.Array children = GetChildren();
        for(int i = 0; i < children.Count; i++){
            if(children[i] is AnimatedSprite){
                anim = (AnimatedSprite) children[i];
            }
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
