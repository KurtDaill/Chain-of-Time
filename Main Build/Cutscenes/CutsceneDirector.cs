using Godot;
using System;
using System.Collections.Generic;
public partial class CutsceneDirector : Node3D
{
    public bool enabled = true;
    public Dictionary<string, Actor> cast;

    public override void _Ready()
    {
        base._Ready();
        foreach(Node node in this.GetChildren()){
            switch(node.GetScript()){

            }
        }
    }
    public void StartCutscene(){

    }
}

public static class CutsceneUtils{
	public enum TextEffectType{
        Bold,
        Italics,
        Underline,
        Wave,
        Impact,
        BigImpact,
        Shiver
    }
}
