using Godot;
using System;

public partial class ExplorePlayer : CharacterBody2D
{
    ExplorePlayerState state = new ExplorePlayerStateFree();
    public Vector3 velocity = new Vector3(0,0,0);
    public int direction;
    public AnimatedSprite2D anim;
    [Export]
    public float moveSpeed = 2;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Finds Relevant Children
        Godot.Collections.Array<Godot.Node> children = GetChildren();
        GetNode("/root/Game").Connect("PlayerWakeEventHandler",new Callable(this,nameof(_on_Game_PlayerWakeEventHandler)));
    }

    public override void _Process(double delta)
    {
        state.HandleInput(this);
        state.Process(this);
    }

    public void _on_Game_PlayerWakeEventHandler(){
        state = new ExplorePlayerStateFree();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(double delta)
//  {
//      
//  }
}
