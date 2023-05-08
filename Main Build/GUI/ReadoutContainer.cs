using Godot;
using System;
//using static PMBattleUtilities;
public partial class ReadoutContainer : VBoxContainer
{   
/*    
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //EmitSignal(nameof(ReadyToPopulateReadouts), this);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(double delta)
//  {
//      
//  }
    

    //[Signal]
    //public delegate void ReadyToPopulateReadouts(ReadoutContainer readouts); 

    public void Reorder(){
        var readouts = this.GetChildren();
        foreach(Node element in readouts){
            if(element != null && element is PlayerCharacterReadout){
                var read = (PlayerCharacterReadout) element;
                switch(read.character.myPosition){
                    case BattlePos.HeroOne :
                        MoveChild(read, 0);
                        break;
                    case BattlePos.HeroTwo :
                        MoveChild(read, 1);
                        break;
                    case BattlePos.HeroThree :
                        MoveChild(read, 2);
                        break;
                }
            }
            else readouts.Remove(element);
        }
    }
    */
}
