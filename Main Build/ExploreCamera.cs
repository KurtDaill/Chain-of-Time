using Godot;
using System;

public class ExploreCamera : Camera
{
    public AnimationPlayer animPlayer;

    public override void _Ready()
    {
        animPlayer = (AnimationPlayer) GetNode("AnimationPlayer");
    }

    public void StartCameraAction(string action){
        animPlayer.Play(action);
    }

    public void _on_AnimationPlayer_animation_finished(string animName){
        EmitSignal(nameof(Camera_Action_Complete));
    }

    [Signal]
    public delegate void Camera_Action_Complete();
}
