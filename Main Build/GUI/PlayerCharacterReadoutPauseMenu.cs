using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerCharacterReadoutPauseMenu : PlayerCharacterReadout{
    TextureRect pointer;
    public override void _EnterTree()
    {
        base._EnterTree();
        pointer = this.GetNode<TextureRect>("Pointer");
    }
    public override void _Ready()
    {
        base._Ready();
        pointer = this.GetNode<TextureRect>("Pointer");
        pointer.Visible = false;
    }
    public override void Select()
    {
        pointer.Visible = true;
        this.highlight.Visible = true;
    }
    public override void Deselect()
    {
        pointer.Visible = false;
        this.highlight.Visible = false;
    }
}