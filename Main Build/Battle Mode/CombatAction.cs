using System;
using Godot;
using System.Collections.Generic;

public abstract partial class CombatAction : Node
{
    protected string name;
    protected string animation;
    public static readonly List<string> noAnimationAbilities = new List<string>(){"SWAP"};
    protected Combatant[] target;
    protected Combatant source;

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

    public CombatEventData ReadyOnCombatantAndGetData(){
        source.ReadyAction(this);
        return GetEventData();
    }

    public virtual void Activate(int phase){
        //Custom Functionality is added here
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