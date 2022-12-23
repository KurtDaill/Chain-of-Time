using Godot;
using System;

public partial class FTB_Teleporter : Area3D
{
    [Export]
    private NodePath target;
    [Export]
    private NodePath cameraPath;
    private ExploreCamera camera;
    private Node3D targetSpatial;

    private ExplorePlayer3D teleportee;

    private bool onStandBy = false;


    public override void _Ready()
    {
        targetSpatial = (Node3D) GetNode(target);
        camera= (ExploreCamera) GetNode(cameraPath);
    }

    public void GoToStandby(){
        onStandBy = true;
    }

    private void OnBodyEnter(Node body){
        GD.Print("HEY!");
        if(body is ExplorePlayer3D){
            if(onStandBy){
                onStandBy = false;
                return;
            }
            camera.StartCameraAction("Fade to Black");
            camera.Connect("Camera_Action_Complete",new Callable(this,nameof(FinishTeleport)));
            teleportee = (ExplorePlayer3D) body;
            if(targetSpatial is FTB_Teleporter){
                FTB_Teleporter temp = (FTB_Teleporter) targetSpatial;
                temp.GoToStandby();
            }
            //animPlayer.Connect("Camera_Action_Complete", this, )
            //Cut to Black,
            //Once We've Cut To Black -> Players Transform3D = Target;
            //Once We've Teleported the Player -> Fade Up From Black
        }
    }

    private void FinishTeleport(){
        teleportee.Position = targetSpatial.Position;
        camera.StartCameraAction("Fade Up");
        camera.Disconnect("Camera_Action_Complete",new Callable(this,nameof(FinishTeleport)));
    }

}
