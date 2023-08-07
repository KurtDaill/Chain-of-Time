using System;
using Godot;
[Tool]
public partial class SidebarStyleDialogueBox : Control{
    [Export(PropertyHint.File)]
    string TextLabelPrefab;
    [Export(PropertyHint.File)]
    string SpacerPrefab;
    PackedScene TextLabelPackedScene;
    PackedScene SpacerPackedScene;
    [Export]
    Node myResponseBox;
	CutsceneResponseBox responses;
    VBoxContainer scrollingDialogueBox;

    
	[Signal]
	public delegate void DisplayFinishedEventHandler();

    public override void _Ready()
    {
        base._Ready();
        TextLabelPackedScene = GD.Load<PackedScene>(TextLabelPrefab);
        SpacerPackedScene = GD.Load<PackedScene>(SpacerPrefab);
		//responses = (CutsceneResponseBox) myResponseBox;
        scrollingDialogueBox = this.GetNode<VBoxContainer>("Scrolling Dialogue");
        responses = (CutsceneResponseBox) myResponseBox;
        responses.Visible = false;
        foreach(Node child in scrollingDialogueBox.GetChildren()){
            child.Free();
        }
    }

	public void BeginDialogue(CutsceneLine line){
        scrollingDialogueBox.AddChild(SpacerPackedScene.Instantiate());
        RichTextLabel dialgoueLabel = (RichTextLabel) TextLabelPackedScene.Instantiate();
        dialgoueLabel.Text = "[b]" + line.GetSpeaker() + "[/b] - " + line.GetText();
        scrollingDialogueBox.AddChild(dialgoueLabel);
	}

	public CutsceneResponseBox StartDialogueResponse(CutsceneDialogueResponse[] responseObjects){
		this.Visible = true;
    	responses.PopulateResponsesAndShow(responseObjects);
		return responses;
	}
}