using Godot;
using System;
using System.Collections.Generic;

public partial class Building : Node3D
{
    bool hidden = false;
    List<StandardMaterial3D> materials;
    bool fadingOut, fadingIn;

    bool destroyed = false;

    float alphaValueWhileTransparent = 0.6F;
    public override void _Ready(){
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
}
