using Godot;
using System;
using System.Collections.Generic;
using static PMBattleUtilities;
using static BattleMenu;

public class PMBattle : Node
{
    private enum TurnPhase{
        Upkeep,
        PlayerMenu,
        PlayerAction,
        TurnOverPause,
        EnemyAction

    }

    [Export]
    private NodePath battleGUI;
    [Export]
    private NodePath debugPlayerOne;
    private PMBattleGUI gui;

    TurnPhase currentPhase;

    List<PMStatus> upkeepEffects = new List<PMStatus>();
    Stack<PMStatus> effectStack;
    PMPlayerCharacter[] playerCharacters = new PMPlayerCharacter[3];
    PMEnemyCharacter[] enemyCharacters = new PMEnemyCharacter[3];

    Dictionary<Targeting, PMCharacter> TargetingLookup = new Dictionary<Targeting, PMCharacter>();
    Queue<PMCharacter> enemyBench = new Queue<PMCharacter>();

    Queue<PMPlayerAbility> playerAttacks;
    Queue<PMBattleAbility> enemyAttacks;
    public override void _Ready()
    {
        //Normal Goto upkeep functions
        currentPhase = TurnPhase.Upkeep;
        effectStack = new Stack<PMStatus>(upkeepEffects);
        gui = (PMBattleGUI) GetNode(battleGUI);
        playerCharacters[0] = GetNode<PMPlayerCharacter>(debugPlayerOne);
    }

    public override void _Process(float delta)
    {
        switch(currentPhase){
            case TurnPhase.Upkeep :
                if(upkeepEffects.Count == 0){ //If there's no more effects to resolve, continue
                    currentPhase = TurnPhase.PlayerMenu;
                    playerAttacks = new Queue<PMPlayerAbility>();
                    gui.ResetGUIState();
                    break;            
                } 

                if(effectStack.Peek().Execute()){ //Execute the Effect, if it's done...
                    effectStack.Pop();  //Remove it  
                }
                break;
            case TurnPhase.PlayerMenu :
                //Send Input to the Battle GUI
                gui.currentMenu.HandleInput(GetPlayerInput(),out PMPlayerAbility ability);
                if(ability != null){//Receive a Player Attack or Null
                    playerAttacks.Enqueue(ability);
                    if(gui.playerCharacterSelected == 3 || playerCharacters[gui.playerCharacterSelected + 1] == null){ //If there's no other character to input commands for...
                        currentPhase = TurnPhase.PlayerAction;
                        playerAttacks.Peek().Begin();
                    }
                }
                break;
            case TurnPhase.PlayerAction : 
                //Peek Player Attack Stack, Execute, get notice whether the attack is still running or not
                if(playerAttacks.Peek().CheckForCompletion()){
                    playerAttacks.Dequeue();
                    if(playerAttacks.Count == 0){
                        currentPhase = TurnPhase.TurnOverPause;
                    }else{
                        playerAttacks.Peek().Begin();
                    }
                }
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

    public PMPlayerCharacter GetPlayerCharacter(int index){
        return playerCharacters[index];
    }

    public PMEnemyCharacter GetEnemyCharacter(int index){
        return enemyCharacters[index];
    }

    public MenuInput GetPlayerInput(){
        if(Input.IsActionJustPressed("ui_up")){
            return BattleMenu.MenuInput.Up;
        }else if(Input.IsActionJustPressed("ui_right")){
            return BattleMenu.MenuInput.Right;
        }else if(Input.IsActionJustPressed("ui_down")){
            return BattleMenu.MenuInput.Down;
        }else if(Input.IsActionJustPressed("ui_left")){
            return BattleMenu.MenuInput.Left;
        }else if(Input.IsActionJustPressed("ui_back")){
            return BattleMenu.MenuInput.Back;
        }else if(Input.IsActionJustPressed("ui_accept")){
            return BattleMenu.MenuInput.Select;
        }else{
            return BattleMenu.MenuInput.None;
        }
    }
}


public static class PMBattleUtilities{

    public static NodePath pathToBattle = "/root";

    public enum AbilityAlignment{
        Normal = 0,
        Magic = 1,
        Tech = 2
    }

    public enum EffectType{
        None = 0,
        Damage = 1,
        Status = 2,
        Healing = 3
    }

    public enum Targeting{                                                                                                                                                                         
        HeroOne = 0b_001000,
        HeroTwo = 0b_010000,                                                                            
        HeroThree = 0b_100000,
        EnemyOne = 0b_000100,
        EnemyTwo = 0b_000010,
        EnemyThree = 0b_000001
    }                                                                                                       

    [Flags]
    public enum TargetingRule{
        None = 0,
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
        None = 0,
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
