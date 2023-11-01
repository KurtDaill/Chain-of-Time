using System;
using Godot;

public partial class ShopButton: Sprite3D{
    private Sprite3D highlightSprite;
    private Label3D text;
    
    [Export]
    private string titleText;
    [Export]
    private string descriptionText;
    [Export]
    private string activateString;

    public override void _Ready()
    {
        base._Ready();
        highlightSprite = this.GetNode<Sprite3D>("Highlight");
        if(highlightSprite == null) throw new ArgumentException("Highlight not found for ShopButton: " + this.Name);
        text = this.GetNode<Label3D>("Text");
        if(text == null) throw new ArgumentNullException("Text not found for ShopButton: " + this.Name);
    }

    public void SetSelect(bool selected){
        this.highlightSprite.Visible = selected;
    }

    public string GetActivateString(){
        return activateString;
    }
    public string GetDescriptionText(){
        return descriptionText;
    }
    public string GetTitleText(){
        return titleText;
    }
}