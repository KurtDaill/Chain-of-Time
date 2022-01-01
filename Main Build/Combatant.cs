using Godot;
using System;

public class Combatant : KinematicBody2D
{
    public Vector2 velocity = Vector2.Zero;
    
    [Export]
    public float moveSpeed = 20;

    public int direction = 0;
    
    public AnimatedSprite anim;

    protected BattlePlayerState StandbyState;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
