using Godot;
using System;

public class BattleManager : Node
{
    [Export]
    private BattlePlayer[] Heroes = new BattlePlayer[3];
    [Export]
    private Combatant[] Enemies = new Combatant[3];

    private RemoteTransform2D[] HeroSpots = new RemoteTransform2D[3];

    private delegate BattleState HandleInputDelegate(BattleManager bm);
    private delegate BattleState ProcessDelegate(BattleManager bm);

    private HandleInputDelegate HandleInput;
    private ProcessDelegate Process;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BattleState temp = new BattleStateStandby();
        HandleInput = temp.HandleInput;
        Process = temp.Process;

        int curHeroSpot = 0;
        Godot.Collections.Array children = GetChildren();
        for(int i = 0; i < children.Count; i ++){
            if(children[i] is RemoteTransform2D){
                HeroSpots[curHeroSpot] = (Godot.RemoteTransform2D) children[i];
                curHeroSpot ++;
            }
        }
    }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
     BattleState tempNewState = null;
     
     tempNewState = HandleInput(this);
    
     if(tempNewState != null){ //BattleState functions return a new state when appropriate, if it returns one...
         HandleInput = tempNewState.HandleInput; //We set Handle Input to the new function,
         Process(this); //We Run Process one last time before we reset it,
         Process = tempNewState.Process; //Then we reset it.
         return;
     }

     tempNewState = Process(this);
     if(tempNewState != null){ //We reset the functions as above, without running Process because we just did.
         HandleInput = tempNewState.HandleInput;
         Process = tempNewState.Process;
    }
    
    }
}
