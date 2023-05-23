using Godot;
using System;

public partial class CameraManager : Node
{
	public void SwitchCamera(Camera3D newCamera){
		GetTree().Root.GetCamera3D().Current = false;
		newCamera.Current = true;
	}
}
