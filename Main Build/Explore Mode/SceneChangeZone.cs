using Godot;
using System;

public partial class SceneChangeZone : InteractZone
{
    [Export(PropertyHint.File)]
    string targetScene;
    protected override void PlayerEnterAreaBehaviour(){
        GetTree().ChangeSceneToFile(targetScene);
    }
}
