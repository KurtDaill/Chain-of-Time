using Godot;
using System;

public class ExplorePlayer : KinematicBody2D
{
    ExplorePlayerState state = new ExplorePlayerStateFree();
    public Vector3 velocity = new Vector3(0,0,0);
    public int direction;
    public AnimatedSprite anim;
    [Export]
    public float moveSpeed = 2;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Finds Relevant Children
        Godot.Collections.Array children = GetChildren();
        GetNode("/root/Game").Connect("PlayerWake", this, nameof(_on_Game_PlayerWake));
    }

    public override void _Process(float delta)
    {
        state.HandleInput(this);
        state.Process(this);
    }

    public void _on_Game_PlayerWake(){
        state = new ExplorePlayerStateFree();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
