using System;
using Godot;
using System.Collections.Generic;

public abstract partial class CombatAction : Node
{
    [Signal]
    public delegate void ActionCompleteEventHandler();
    protected string name;
    protected string animation;
    protected bool running = false;
    public static readonly List<string> noAnimationAbilities = new List<string>(){"SWAP"};
    protected Combatant[] target;
    protected Combatant source;

    //The 0 entry of this array is always reserved for the core animation of this combat action
    protected bool[] flagsRequiredToComplete = new bool[1]{false};

    //Should be spawned in as a child of proposedSource
    public virtual void Setup(Combatant proposedSource){
        source = proposedSource;
        if(!source.GetChildren().Contains(this)){
            GetTree().Quit();
            throw new BadActionSetupException("CombatAction must be child of combatant it is being setup with!");
        }
        if(!source.HasAnimation(animation)){
            //If this abiltiy isn't on the list of Abilities that don't need an animation on their source
            if(!noAnimationAbilities.Contains(this.name)){
                GetTree().Quit();
                throw new BadActionSetupException("Combatant must have an animation with a name the same as this Action's animation field!");
            }
        }
    }

    public string GetName(){
        return name;
    }

    public CombatEventData GetEventData(){
        return new CombatEventData(animation, source, this);
    }

    public virtual void Run(){
        for(int i = 0; i < flagsRequiredToComplete.Length; i++){
            flagsRequiredToComplete[i] = false;
        }
        running = true;
        WaitForOwnAnimation();
    }

    public virtual void Activate(int phase){
        WaitForOwnAnimation();
        //Custom Functionality is added here
    }

    public override void _Process(double delta){
        if(!running) return;
        bool complete = true;
        foreach(bool flag in flagsRequiredToComplete){
            if(flag == false){
                complete = false;
                break;
            }
        }
        if(complete){
            EmitSignal(CombatAction.SignalName.ActionComplete);
            running = false;
        } 
    }
    
    protected class BadActionSetupException : Exception
    {
        public BadActionSetupException()
        {
        }

        public BadActionSetupException(string message)
            : base(message)
        {
        }

        public BadActionSetupException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    protected class BadAbilityExecuteCallException : Exception
    {
        public BadAbilityExecuteCallException()
        {
        }

        public BadAbilityExecuteCallException(string message)
            : base(message)
        {
        }

        public BadAbilityExecuteCallException(string message, Exception inner)
            : base(message, inner)
        {
        }
        
    }
    
    public virtual async void WaitForOwnAnimation(){
        await ToSignal(this.source.GetAnimationPlayer(), AnimationPlayer.SignalName.AnimationFinished);
        flagsRequiredToComplete[0] = true;
    }
}