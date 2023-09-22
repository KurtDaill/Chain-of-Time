using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class ExploreCamera : Camera3D
{
    public ExplorePlayer player;
    public Building hiddenBuilding;
    public override void _Ready(){
        player = this.GetParent<ExplorePlayer>();
    }
    public override void _Process(double delta){
        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(this.GlobalPosition, player.GlobalPosition, 4096);
        query.CollideWithAreas = true;
        var result = spaceState.IntersectRay(query);
        if(result.TryGetValue("collider", out Variant collider)){
            Building building = (Building) ((Area3D)collider).GetParent();
            if(hiddenBuilding != null && building != hiddenBuilding){
                hiddenBuilding.ShowBuilding();
            }
            hiddenBuilding = building;
            hiddenBuilding.HideBuilding();
        }else{
            if(hiddenBuilding != null){
                hiddenBuilding.ShowBuilding();
                hiddenBuilding = null;
            }
        }
    }
}
