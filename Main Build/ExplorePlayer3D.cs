using Godot;
using System;

public class ExplorePlayer3D : KinematicBody
{
    private float deltaZ, deltaX;
    private Vector3 move;
    [Export]
    private float moveSpeed;
    [Export(PropertyHint.Range, "0,1")]
    private float drag;

    public override void _Ready()
    {
        deltaZ = 0;
        deltaX = 0;
    }
    public override void _Process(float delta)
    {
        if(Input.IsActionPressed("ui_right")){
            deltaZ = -1;
            
        }else if(Input.IsActionPressed("ui_left")){
            deltaZ = 1;
        }

        if(Input.IsActionPressed("ui_up")){
            deltaX = -1;
        }else if(Input.IsActionPressed("ui_down")){
            deltaX = 1;
        }
        deltaZ = deltaZ * (1-drag);
        deltaX = deltaX * (1-drag);
        move = new Vector3(deltaX, 0, deltaZ) * moveSpeed;
        MoveAndCollide(move);
    }
}
