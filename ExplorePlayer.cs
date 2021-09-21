using Godot;
using System;

public class ExplorePlayer : KinematicBody2D
{
    ExplorePlayerState state = new ExplorePlayerStateFree();
    public Vector2 velocity = new Vector2(0,0);
    public int direction;
    public AnimatedSprite anim;
    [Export]
    public float moveSpeed = 2;
    // Called when the node enters the scene tree for the first time.
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

    public override void _Process(float delta)
    {
        state.HandleInput(this);
        state.Process(this);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
