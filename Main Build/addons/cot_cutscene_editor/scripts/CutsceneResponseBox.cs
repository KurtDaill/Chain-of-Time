using System;
using Godot;

public partial class CutsceneResponseBox : VBoxContainer{
    [Export]
    private RichTextLabel[] responseLabels;
    private CutsceneDialogueResponse[] responseObjects; 
    int selectedResponse;
    int numberOfValidResponses;

    public void PopulateResponsesAndShow(CutsceneDialogueResponse[] responsesToPopulateInBox){
        numberOfValidResponses = responsesToPopulateInBox.Length;
        responseObjects = new CutsceneDialogueResponse[numberOfValidResponses];
        for(int i = 0; i < Math.Min(4, numberOfValidResponses); i++){
            responseLabels[i].Text = responsesToPopulateInBox[i].GetResponseText();
            responseObjects[i] = responsesToPopulateInBox[i];
        }
        selectedResponse = 0;
        this.Visible = true;
        SetResponseGUI();
    }

    //Returns the name of the block the current response is pointed at (or "EXIT")
    public void GoUpList(){
        if(selectedResponse > 0) selectedResponse--;
        SetResponseGUI();
    }

    //Returns the name of the block the current response is pointed at (or "EXIT")
    public void GoDownList(){
        if(selectedResponse < numberOfValidResponses-1) selectedResponse++;
        SetResponseGUI();
    }   

    public string GetFinalDestination(){
        string output = responseObjects[selectedResponse].GetTargetBlock();
        responseObjects = null;
        this.Visible = false;
        return output;
    }

    private void SetResponseGUI(){
        for(int i = 0; i < 4; i++){
            if(i >= numberOfValidResponses){responseLabels[i].Visible = false;} //Hide response labels we aren't using
            responseLabels[i].GetNode<Panel>("Highlight").Visible = i == selectedResponse; //If the response is the one that's selected, show the highlight, otherwise hide the highlight
        }
    }
}