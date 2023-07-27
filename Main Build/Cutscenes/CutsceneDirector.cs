using Godot;
using System;
using System.Collections.Generic;
public partial class CutsceneDirector : Node3D
{
    public bool enabled = true;
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
