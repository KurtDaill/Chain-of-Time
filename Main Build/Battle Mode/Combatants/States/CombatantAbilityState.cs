using Godot;
using System;
using System.Collections.Generic;
using static AbilityUtilities;

public abstract class CombatantAbilityState : CombatantState{
    public AbilityQualities qualities = AbilityQualities.None;
    public float activationDelay = 0;
}

public static class AbilityUtilities
{
    public enum AbilityQualities{
        None = 0b_0000_0000_0000,
        MeleeAttack = 0b_0000_0000_0001,
        RangedAttack = 0b_0000_0000_0010,
        Tech = 0b_0000_0000_0100,
        Magic = 0b_0000_0000_1000,
        HitsAllEnemies = 0b_0000_0001_0000,
        BuffSelf = 0b_0000_0010_0000,
        BuffOthers = 0b_0000_0100_0000,
        DebuffsEnemy = 0b_0000_1000_0000,
        DebuffsSelf = 0b_0001_0000_0000,
        DebuffsFriends = 0b_0010_0000_0000,
        Dodgable = 0b_0100_0000_0000,
    }

    public enum PlayerAbilityType{
        Normal = 0b_0000_0001,
        Spell = 0b_0000_0010,
        Tech = 0b_0000_0100,
        Skill = 0b_0001_0000,
        Attack = 0b_0010_0000
    }

    public static void CheckForRequiredAnimations(Combatant parent, string[] reqArr){ //TODO: Revisit this
        return;
        /*
        bool passing = true;
        List<string> missingAnims = new List<string>();
        AnimationNodeStateMachine sm = (AnimationNodeStateMachine) parent.animTree.Get("parameters/playback");
        foreach(string req in reqArr){
            if(!sm.HasNode(req)){
                passing = false;
                missingAnims.Add(req);
            }
        }
        if(!passing){
            throw new MissingAbilityAnimationException(missingAnims);
        }
        */
    }
}

public class MissingAbilityAnimationException : Exception
{
    public MissingAbilityAnimationException()
    {
    }

    public MissingAbilityAnimationException(string message)
        : base(message)
    {
    }

    public MissingAbilityAnimationException(List<string> missingAnims): base("Animations Missing for Enemy Ability \n check Godot logs for list of missing anims")
    {
        foreach(string miss in missingAnims){
            GD.Print("Missing Animation: " + miss);
        }
    }

    public MissingAbilityAnimationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}