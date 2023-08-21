using Godot;
using System;

public partial class InteractZone : Area3D
{
    [Export]
    GameplayMode targetMode;
    //Possible to have things other than modes we can move to? Priority system?
    public override void _Ready(){
        this.BodyEntered += OnBodyEntered;
        this.BodyExited += OnBodyExited;
    }
    public void OnBodyEntered(Node3D body){
        if(body is ExplorePlayer){
            PlayerEnterAreaBehaviour();
        }
    }
    public void OnBodyExited(Node3D body){
        if(body is ExplorePlayer){
            PlayerExitAreaBehaviour();
        }
    }
    
    public virtual GameplayMode Activate(){
        return targetMode;
    }

    protected virtual void PlayerEnterAreaBehaviour(){
        return;
    }
    protected virtual void PlayerExitAreaBehaviour(){
        return;
    }
}
