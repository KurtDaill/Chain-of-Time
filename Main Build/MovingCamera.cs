using Godot;
using System;

public partial class MovingCamera : Camera3D
{
    Transform3D target;

    public Transform3D baseTransform;
    double speed =  1;
    double timer = 2;

    enum CameraState{
        Standby,
        InterpToTrans
    }

    CameraState state = CameraState.Standby;

    public override void _Ready(){
        baseTransform = this.Transform;
    }
    public override void _Process(double delta)
    {
        switch(state){
            case CameraState.Standby: 
                break;
            case CameraState.InterpToTrans:
                this.Transform = this.Transform.InterpolateWith(target, (float)delta * (float)speed);
                timer -= delta;
                if(timer <= 0){
                    this.Transform = target;
                    state = CameraState.Standby;
                }
                break;
        }
    }
    public void InterpolateToTransform(Transform3D targetTransform, float speedFactor = 1, float timer = 1){
        target = targetTransform;
        speed = speedFactor;
        state = CameraState.InterpToTrans;
        this.timer = timer;
    }


}
