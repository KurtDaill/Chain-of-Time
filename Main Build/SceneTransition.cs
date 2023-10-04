using Godot;
using System;
using System.Threading.Tasks;

public partial class SceneTransition : GameplayMode {
    [Export(PropertyHint.File)]
    string targetScenePath;

    
    /*This function is used for the current mode to set everything up how it would like it before having to accepting Player Input.
    It's configured as a task such that visual effects lasting longer than a frame can be handled under this function.*/
    public override Task StartUp(GameplayMode oldMode){
        GetTree().ChangeSceneToFile(targetScenePath);
        return null;
    }
}