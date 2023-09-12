using Godot;
using System;

public partial class NPCNavigationRegion : NavigationRegion3D
{
    public override void _Ready(){
        this.BakeNavigationMesh(true);
    }
    
}
