using Godot;
using System;
using System.Collections.Generic;

public class PMBattle : Node
{
    private enum TurnPhase{
        Upkeep,
        PlayerMenu,
        PlayerAction,
        TurnOverPause,
        EnemyAction

    }

    TurnPhase currentPhase;

    List<PMEffect> upkeepEffects = new List<PMEffect>();
    Stack<PMEffect> effectStack;

    Queue<PMBattleAbility> playerAttacks;
    Queue<PMBattleAbility> enemyAttacks;
    public override void _Ready()
    {
        //Normal Goto upkeep functions
        currentPhase = TurnPhase.Upkeep;
        effectStack = new Stack<PMEffect>(upkeepEffects);
    }

    public override void _Process(float delta)
    {
        switch(currentPhase){
            case TurnPhase.Upkeep :
                if(upkeepEffects.Count == 0){ //If there's no more effects to resolve, continue
                    currentPhase = TurnPhase.PlayerMenu;            
                } 

                if(effectStack.Peek().Execute()){ //Execute the Effect, if it's done...
                    effectStack.Pop();  //Remove it  
                }
                break;
            case TurnPhase.PlayerMenu :
                //Send Input to the Battle GUI
                //Receive a Player Attack or Null
                //Once we've received our third, we continue because the Player Attack Stack is done
                break;
            case TurnPhase.PlayerAction : 
                //Peek Player Attack Stack, Execute, get notice whether the attack is still running or not
                //If the attack is finished, check if it was the third attack
                    //If it is, go on to the turn over pause
                    //If not, pop to the next attack
                break;
            case TurnPhase.TurnOverPause : 
                //Wait a second, also ask the enemies what their AI wants them to do.
                //Populate the enemy stack with that.
                break;
            case TurnPhase.EnemyAction :
                //Basically the same loop as PlayerAction but with the enemy stack 
                break;
        }
    }
}

public class PMBattleUtilities{
    public enum AbilityAlignment{
        Normal = 1,
        Magic = 2,
        Tech = 3
    }

    [Flags]
    public enum TargetRule{
        
        Self = 1,
        SingleEnemyMelee = 2,
        SingleEnemyRanged = 3,
        SingleEnemyReach = 4,
        AllEnemy = 5,
        SingleAlly = 6,
        AllAlly = 7,
        AllyOne = 10,
        AllyTwo = 11,
        AllyThree = 12,
        EnemyOne = 13,
        EnemyTwo = 14,
        EnemyThree = 15
    }

    [Flags]
    public enum StatusEffect{
        Burn = 1,
        Freeze = 2,
        Poisoned = 3,
        Stunned = 4,
        Airborne = 5,
        TargetLocked = 6,
        Silenced = 7,
        Busted = 8 
    }
}
