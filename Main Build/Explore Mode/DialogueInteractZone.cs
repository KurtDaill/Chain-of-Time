using Godot;
using System;

public partial class DialogueInteractZone : InteractZone
{
    [Export]
    DialoguePrompt prompt;
    [Export]
    bool isCutscene;

    protected override void PlayerEnterAreaBehaviour(){
        prompt.ShowPrompt();
    }    
    protected override void PlayerExitAreaBehaviour(){
        prompt.HidePrompt();
    }

    public override GameplayMode Activate(){
        if(isCutscene){
            CutsceneDirector cutscene = base.Activate() as CutsceneDirector;
            prompt.HidePromptForCutscene(cutscene);
            return cutscene;
        }else{
            return base.Activate();
        }
    }
}
