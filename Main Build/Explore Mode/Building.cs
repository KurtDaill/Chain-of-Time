using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Building : Node3D
{
    bool hidden = false;
    List<StandardMaterial3D> materials;
    bool fadingOut, fadingIn;

    bool destroyed = false;

    bool beingVandalized = false;

    float alphaValueWhileTransparent = 0.6F;
    public override void _Ready(){
        //Basic Setup, Assigning Objects to Data Structures
        fadingOut = false;
        fadingIn = false;

        materials = new List<StandardMaterial3D>();
        var buildingMeshes = new List<MeshInstance3D>();
        var foundMeshes = this.FindChildren("*", "MeshInstance3D");
        foreach(Node foundMesh in foundMeshes){
            MeshInstance3D mesh = (MeshInstance3D)foundMesh;
            for(int i = 0; i < mesh.Mesh.GetSurfaceCount(); i++){
                if(mesh.Mesh.SurfaceGetMaterial(i) is StandardMaterial3D){
                    mesh.Mesh = mesh.Mesh.Duplicate() as Mesh;
                    StandardMaterial3D newMat = mesh.Mesh.SurfaceGetMaterial(i).Duplicate(false) as StandardMaterial3D;
                    mesh.Mesh.SurfaceSetMaterial(i, newMat);
                    //mesh.Mesh.SurfaceGetMaterial(i).Set("resourcelocalto_scene", true);
                    materials.Add(newMat);
                }else{
                    throw new Exception("Building Class doesn't support materials other than StandardMaterial3D presently!");
                }
            }
            buildingMeshes.Add(mesh);
        }

        //Validating that the Node has all required children, possibly running futher validations.
        //We want to explicitly catch any setup issues with custom exception messages to ease later bug fixes.
        Godot.Collections.Array<Node> children = this.GetChildren();
        if(!children.Any(x => x.Name == "BuildingUndamaged")) throw new Exception("Building Object Missing Child Node: BuildingUndamaged");
        if(!children.Any(x => x.Name == "BuildingDestroyed")) throw new Exception("Building Object Missing Child Node: BuildingDestroyed"); 
        if(!children.Any(x => x.Name == "Fires")) throw new Exception("Building Object Missing Child Node: Fires"); 
        if(!children.Any(x => x.Name == "Lights")) throw new Exception("Building Object Missing Child Node: Lights"); 
        if(!children.Any(x => x.Name == "VandalismPoint")) throw new Exception("Building Object Missing Child Node: VandalismPoint"); 
    }

    //This function is run everytime a night or day starts, and is used for updating and setting states properly.
    public virtual void UpdateState(){
        //If we're destroyed, make the destroyed model visible, otherwise make the normal model visible
        this.GetNode<Node3D>("BuildingUndamaged").Visible = !destroyed;
        this.GetNode<Node3D>("BuildingDestroyed").Visible = destroyed;
    }

    public override void _Process(double delta){
        if(fadingOut && fadingIn) throw new Exception("Error in Building Class: Building set to lose and gain transparency at the same time.");
        if(fadingOut){
            foreach(StandardMaterial3D mat in materials){
                //We Fade out the albedo values every frame until it hits a certain threshold, then we stop fading out.
                mat.AlbedoColor =  new Color(mat.AlbedoColor.R, mat.AlbedoColor.G, mat.AlbedoColor.B, mat.AlbedoColor.A - 0.02F);
                if(mat.AlbedoColor.A <= alphaValueWhileTransparent){
                    mat.AlbedoColor =  new Color(mat.AlbedoColor.R, mat.AlbedoColor.G, mat.AlbedoColor.B, alphaValueWhileTransparent);
                    fadingOut = false;
                }
            }
        }else if(fadingIn){
            foreach(StandardMaterial3D mat in materials){
                //We Fade out the albedo values every frame until it hits a certain threshold, then we stop fading out.
                mat.AlbedoColor =  new Color(mat.AlbedoColor.R, mat.AlbedoColor.G, mat.AlbedoColor.B, Math.Min(mat.AlbedoColor.A * 1.03F, 1));
                if(mat.AlbedoColor.A == 1){
                    fadingIn = false;
                }
            }
        }
    }

    public void StartVandalism(){
        this.GetNode<Node3D>("Fires").Visible = true;
    }

    public Vector3 GetVandalismPoint(){
        return this.GetNode<Marker3D>("VandalismPoint").GlobalPosition;
    }

    public void DestroyMe(){
        this.destroyed = true;
    }

    public void HideBuilding(){
        fadingIn = false; 
        fadingOut = true;
        //this.Visible = false;
    }
    public void ShowBuilding(){
        fadingIn = true;
        fadingOut = false;
        //this.Visible = true;
    }
    public bool IsHidden(){
        return true;
    }

    public bool IsDestroyed(){
        return destroyed;
    }

    public bool IsBeingVandalized(){
        return beingVandalized;
    }
}
