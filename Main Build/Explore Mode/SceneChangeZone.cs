using Godot;
using System;

public partial class SceneChangeZone : InteractZone
{
    [Export(PropertyHint.File)]
    string targetScene;
    [Export]
    string targetLocationInScene = "";
    [Export]
    bool canBeEnteredAtNight = false;
    protected override void PlayerEnterAreaBehaviour(){
        if(!canBeEnteredAtNight){
            if(this.GetNode<GameMaster>("/root/GameMaster").GetMode() is NightDefense){
                return;
            }
        }
        this.GetNode<SceneConfig>("/root/SceneConfig").TransitionToNewScene(targetLocationInScene, targetScene);
    }
}
