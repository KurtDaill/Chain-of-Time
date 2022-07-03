using Godot;
using System;

public class MovingCamera : Camera
{
    Transform target;
    float speed =  1;
    float timer = 2;

    enum CameraState{
        Standby,
        InterpToTrans
    }

    CameraState state = CameraState.Standby;
    public override void _Process(float delta)
    {
        switch(state){
            case CameraState.Standby: 
                break;
            case CameraState.InterpToTrans:
                this.Transform = this.Transform.InterpolateWith(target, delta * speed);
                timer -= delta;
                if(timer <= 0){
                    this.Transform = target;
                    state = CameraState.Standby;
                }
                break;
        }
    }

    private void InterpToTrans(){

    }

    public void InterpolateToTransform(Transform targetTransform, float speedFactor = 1, float timer = 1){
        target = targetTransform;
        speed = speedFactor;
        state = CameraState.InterpToTrans;
    }


}
