using Godot;
using System;
using System.Threading.Tasks;

public partial class ResultsScreen : GameplayMode
{

    public override Task StartUp(GameplayMode oldMode){
        if(oldMode is NightDefense){
            NightDefense previousDefMode = oldMode as NightDefense;
            
        }else{
            throw new ArgumentException("Results Screen can only be accessed from Night Defense Mode!");
        }
    }
}
