using Godot;
using System;
using System.Runtime.InteropServices;
using System.Linq;

public partial class ExploreHUD : Panel
{
    VBoxContainer questVBox;
    PackedScene questTextBoxTemplate;

    public override void _Ready(){
        questVBox = GetNode<VBoxContainer>("Quests");
        questTextBoxTemplate = new PackedScene();
        Error response = questTextBoxTemplate.Pack(questVBox.GetNode("Quest Box Template"));
        if(response == Error.Ok){
            questVBox.GetNode("Quest Box Template").QueueFree();
        }else{
            throw new ExternalException("Godot didn't like the Quest Text Template, returning Error Code: " + response);
        }
        GetNode<QuestMaster>("/root/QuestMaster").QuestsUpdated += OnQuestsUpdate;
        OnQuestsUpdate(GetNode<QuestMaster>("/root/QuestMaster").GetAciveQuests());
    }

    public void OnQuestsUpdate(Quest[] activeQuests){
        Godot.Collections.Array<Node> previousQuestTextBoxes = questVBox.GetChildren();
        for(int i = 2; i < previousQuestTextBoxes.Count; i++){
            previousQuestTextBoxes[i].Free();
        }
        foreach(Quest quest in activeQuests){
            RichTextLabel questTextBox = questTextBoxTemplate.Instantiate<RichTextLabel>();
            if(quest.IsCompleted()) questTextBox.Text = "[u][s]" + quest.GetQuestName() + "[/s][/u]";
            else questTextBox.Text = "[u]" + quest.GetQuestName() + "[/u]";
            foreach(QuestObjective obj in quest.GetNode("Objectives").GetChildren().Where(x => x is QuestObjective)){
                if(obj.IsCompleted()){
                    questTextBox.Text += "\n  -[s]" + obj.GetDescription() + "[/s]";
                }else{
                    questTextBox.Text += "\n  -" + obj.GetDescription();
                }
            }
            questVBox.AddChild(questTextBox);   
        }
    }
}
