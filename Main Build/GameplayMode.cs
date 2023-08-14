using System;
using Godot;
using static GameplayUtilities;
using System.Threading.Tasks;

public abstract partial class GameplayMode : Node3D{

    /*This function is used for the current mode to set everything up how it would like it before having to accepting Player Input.
    It's configured as a task such that visual effects lasting longer than a frame can be handled under this function.*/
    public virtual Task StartUp(){
        return null;
    }

    //Input from the player is hanlded through this function
    public virtual void HandleInput(PlayerInput input){
    }  

    /*Behaviour that proceeds without player input is handled here.
    Because there should only be one mode running at a time, we don't use process with our gameplay modes, but instead
    have the GameMaster call it's current mode's "RemoteProcess" */
    public virtual async Task<GameplayMode> RemoteProcess(double delta)
    {
        return null;
    }

    /*This function is used to wrap up everything after the gameplay mode has completed itself (a cutscene finishes, the player wins combat, etc.).
    
    As this mode knows what it just send out to GameMaster for the next mode, it can perfrom specific clean-up actions and changes, the most
    obvious case being moving the camera to the proper place through a linear transition.*/
    public virtual Task TransitionOut(){
        //Move the Camera Here...
        return null;
    }
}