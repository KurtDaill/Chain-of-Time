using Godot;
using System;

public partial class SceneChangeZone : InteractZone
{
    [Export(PropertyHint.File)]
    string targetScene;
    [Export]
    string targetLocationInScene = "";
    [Export]
    bool saveCityOnExit = false;
    protected override void PlayerEnterAreaBehaviour(){
        if(targetLocationInScene != "") this.GetNode<GameMaster>("/root/GameMaster").SetSpawnPoint(targetLocationInScene);
        if(saveCityOnExit){
            
        }
        GetTree().ChangeSceneToFile(targetScene);
    }
}
