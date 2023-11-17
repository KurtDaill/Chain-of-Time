using Godot;
using System;
using System.Collections.Generic;

public partial class BuildingHider : Node3D
{
	[Export]
    Node3D viewTarget;
	Camera3D cam;
    List<Building> buildingsHiddenByThisCamera;

    //We use this list to see what buildings we've hidden that should be shown because they're no longer between the camera and the player
    List<Building> buildingsToShowThisFrame;
	Godot.Collections.Array<Rid> exclusions;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(this.GetParent() is not Camera3D){
			throw new ArgumentNullException("Building Hider Node Must Be Parented to a Camera!");
		}
		cam = this.GetParent<Camera3D>();
        buildingsHiddenByThisCamera = new();
        buildingsToShowThisFrame = new();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        buildingsToShowThisFrame = buildingsHiddenByThisCamera;
        buildingsHiddenByThisCamera = new();
		exclusions = new();

        var spaceState = GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(cam.GlobalPosition, viewTarget.GlobalPosition, 4096);
        query.CollideWithAreas = true;
        while(true){
            var result = spaceState.IntersectRay(query);
            if(result.TryGetValue("collider", out Variant collider)){
                Building building = (Building) ((Area3D)collider).GetParent();
                building.HideBuilding();
                buildingsToShowThisFrame.Remove(building);
                buildingsHiddenByThisCamera.Add(building);
				result.TryGetValue("rid", out Variant rid);
                exclusions.Add((Rid)rid);
        		query = PhysicsRayQueryParameters3D.Create(cam.GlobalPosition, viewTarget.GlobalPosition, 4096, exclusions);
            }else{
                foreach(Building building in buildingsToShowThisFrame){
                    building.ShowBuilding();
                }
                break;
            }
        }

	}

    public override void _ExitTree()
    {
        base._ExitTree();
        foreach(Building build in buildingsHiddenByThisCamera){
            build.ShowBuilding();
        }
        foreach(Building build in buildingsToShowThisFrame){
            build.ShowBuilding();        
        }
    }
}
