using Godot;
using System;

public partial class CutsceneInteractZone : InteractZone
{
    [Export]
    DialoguePrompt prompt;

    protected override void PlayerEnterAreaBehaviour(){
        prompt.ShowPrompt();
    }    
    protected override void PlayerExitAreaBehaviour(){
        prompt.HidePrompt();
    }
}
