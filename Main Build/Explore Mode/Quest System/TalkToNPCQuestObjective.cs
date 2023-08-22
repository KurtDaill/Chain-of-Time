using Godot;
using System;

public partial class TalkToNPCQuestObjective : QuestObjective
{
    //This is the cutscene that the player has to see to get this quest objective complete
    [Export]
    string targetCutscene;
    public override void ConnectToSignalInMode(GameplayMode mode)
    {
        if(mode is CutsceneDirector){
            CutsceneDirector cutscene = (CutsceneDirector) mode;
            cutscene.CutsceneComplete += OnCutsceneComplete;
        }else{
            throw new ArgumentException("Talk to NPC Quest Objectives can only be connected to Cutscenes!");
        }
    }

    public void OnCutsceneComplete(string cutsceneName){
        if(targetCutscene == cutsceneName){
            completed = true;
            EmitSignal(QuestObjective.SignalName.ObjectiveComplete);
        }
    }
}
