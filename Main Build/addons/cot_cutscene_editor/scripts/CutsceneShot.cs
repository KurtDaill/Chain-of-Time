using System;
using Godot;

public partial class CutsceneShot : Camera3D{
    public ShotDetails GetShotDetails(){
        return new ShotDetails(
            this.GlobalTransform,
            this.Fov,
            this.Size
        );
    }
}

public class ShotDetails{
    Transform3D globalTransform;
    float fov;
    float size;

    public ShotDetails(Transform3D globalTransform, float fov, float size){
        this.globalTransform = globalTransform;
        this.fov = fov;
        this.size = size;
    }

    public Transform3D GetGlobalTransform(){return globalTransform;}
    public float GetFieldOfView(){return fov;}
    public float GetSize(){return size;}
}