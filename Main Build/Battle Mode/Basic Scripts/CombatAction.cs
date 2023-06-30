using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
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

    protected Battle parentBattle;

    //The 0 entry of this array is always reserved for the core animation of this combat action
    protected bool[] flagsRequiredToComplete = new bool[1]{false};

    //Should be spawned in as a child of proposedSource
    public virtual void Setup(Combatant proposedSource){
        source = proposedSource;
        if(!source.GetChildren().Contains(this)){
            GetTree().Quit();
            throw new BadActionSetupException("CombatAction must be child of combatant it is being setup with!");
        }
        if(animation != null && !source.HasAnimation(animation)){
            if(!noAnimationAbilities.Contains(this.name)){ //If this abiltiy isn't on the list of Abilities that don't need an animation on their source
                GetTree().Quit();
                throw new BadActionSetupException("Combatant must have an animation with a name the same as this Action's animation field!");
            }
        }
    }

    public override void _Process(double delta){
        if(running){
            //If there are no flags we're waiting on
            if(flagsRequiredToComplete.Where(x => x == false).Count() == 0){
                EmitSignal(CombatAction.SignalName.ActionComplete);
                running = false;
            } 
        }
    }

    public string GetName(){
        return name;
    }

    public CombatEventData GetEventData(){
        return new CombatEventData(animation, source, this);
    }
    
    public void SetBattle(Battle battle){
        parentBattle = battle;    
    }

    public virtual void Begin(){
        for(int i = 0; i < flagsRequiredToComplete.Length; i++){
            flagsRequiredToComplete[i] = false;
        }
        running = true;
    }

    public virtual void AnimationTrigger(int phase){} //Custom Functionality is added here

    public virtual async void ListenForAnimationFinished(){
        await ToSignal(this.source.GetAnimationPlayer(), AnimationPlayer.SignalName.AnimationFinished);
        flagsRequiredToComplete[0] = true;
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
}