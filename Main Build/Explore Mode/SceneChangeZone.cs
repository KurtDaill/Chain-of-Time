using Godot;
using System;

public partial class SceneChangeZone : InteractZone
{
    [Export(PropertyHint.File)]
    string targetScene;
    [Export]
    string targetLocationInScene = "";
    protected override void PlayerEnterAreaBehaviour(){
        this.GetNode<SceneConfig>("/root/SceneConfig").TransitionToNewScene(targetLocationInScene, targetScene);
    }
}
